using AllBirds.Application.Contracts;
using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AllBirds.Application.Services.ColorServices
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IMapper _mapper;

        public ColorService(IColorRepository colorRepository, IMapper mapper)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
        }

        public async Task<ResultView<CUColorDTO>> CreateAsync(CUColorDTO cUColorDTO)
        {
            ResultView<CUColorDTO> resultView = new();
            try
            {
                bool Check = (await _colorRepository.GetAllAsync()).Any(P => P.Id == cUColorDTO.Id || P.NameEn == cUColorDTO.NameEn || P.NameAr == cUColorDTO.NameAr || P.Code == cUColorDTO.Code);
                if (Check)
                {
                    resultView.Data = cUColorDTO;
                    resultView.IsSuccess = false;
                    resultView.Msg = $"This Color {cUColorDTO.NameEn} Already Exist";
                    return resultView;
                }
                Color mappedColor = _mapper.Map<Color>(cUColorDTO);
                Color createdColor = await _colorRepository.CreateAsync(mappedColor);
                if (mappedColor is not null)
                {
                    await _colorRepository.SaveChangesAsync();
                    CUColorDTO cUColor = _mapper.Map<CUColorDTO>(createdColor);
                    resultView.IsSuccess = true;
                    resultView.Data = cUColor;
                    resultView.Msg = $"Color {cUColor.NameEn} Created Successfully";
                    return resultView;
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Color {mappedColor.NameEn} Not Created Successfully";
                    return resultView;
                }

            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Color ${cUColorDTO.NameEn} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUColorDTO>> UpdateAsync(CUColorDTO cUColorDTO)
        {
            ResultView<CUColorDTO> resultView = new();
            try
            {
                Color? colorObj = (await _colorRepository.GetAllAsync()).FirstOrDefault(s => s.Id == cUColorDTO.Id && s.IsDeleted == false);
                if (colorObj is not null)
                {
                    colorObj.NameEn = cUColorDTO.NameEn;
                    colorObj.Code = cUColorDTO.Code;
                    await _colorRepository.SaveChangesAsync();
                    CUColorDTO cUColor = _mapper.Map<CUColorDTO>(colorObj);
                    resultView.IsSuccess = true;
                    resultView.Data = cUColor;
                    resultView.Msg = $"Color {cUColor.NameEn} Updated Successfully";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "This Color Not Found";
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Color ${cUColorDTO.NameEn} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<CUColorDTO> SoftDeleteAsync(int colorId)
        {
            Color? colorObj = (await _colorRepository.GetAllAsync()).FirstOrDefault(s => s.Id == colorId && s.IsDeleted == false);
            if (colorObj is not null)
            {
                colorObj.IsDeleted = true;
                await _colorRepository.SaveChangesAsync();
                return _mapper.Map<CUColorDTO>(colorObj);
            }
            return null;
        }
        public async Task<ResultView<CUColorDTO>> HardDeleteAsync(int colorId) // will this throw tracking exception ??
        {
            ResultView<CUColorDTO> resultView = new();
            try
            {
                bool dependentPrds = (await _colorRepository.GetAllAsync()).Any(c => c.Id == colorId && c.Products.Count > 0);
                if (dependentPrds)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = "Cannot Delete This Color As There Are Product Color Variants That Depend On It";
                    return resultView;
                }
                Color? colorObj = (await _colorRepository.GetAllAsync()).FirstOrDefault(s => s.Id == colorId);
                if (colorObj is not null)
                {
                    Color deletedColor = await _colorRepository.DeleteAsync(colorObj);
                    await _colorRepository.SaveChangesAsync();
                    CUColorDTO cUColor = _mapper.Map<CUColorDTO>(deletedColor);
                    resultView.IsSuccess = true;
                    resultView.Data = cUColor;
                    resultView.Msg = $"Color {cUColor.NameEn} Deleted Successfully";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "This Color Not Found";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetch Colors ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<List<CUColorDTO>>> GetAllAsync()
        {
            ResultView<List<CUColorDTO>> resultView = new();
            try
            {
                List<Color> colorsList = [.. (await _colorRepository.GetAllAsync()).Where(s => !s.IsDeleted)];
                List<CUColorDTO> cUColors = _mapper.Map<List<CUColorDTO>>(colorsList);
                if (cUColors.Count > 0)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = cUColors;
                    resultView.Msg = "All Colors Fetched Successfully";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = "Colors Is Empty..! Sorry";
                resultView.Data = null;
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetch Colors ${ex.Message}";
            }
            return resultView;
        }

        public async Task<List<CUColorDTO>> GetAllWithDeletedAsync()
        {
            List<Color> colorsList = [.. (await _colorRepository.GetAllAsync())];
            return _mapper.Map<List<CUColorDTO>>(colorsList);
        }
        public async Task<CUColorDTO> GetByIdAsync(int colorId)
        {
            IQueryable<Color> ColorList = await _colorRepository.GetAllAsync();
            Color? ColorObj = ColorList.FirstOrDefault(s => s.Id == colorId && !s.IsDeleted); // bool operator
            if (ColorObj != null)
            {
                return _mapper.Map<CUColorDTO>(ColorObj);
            }
            return null;
        }
    }
}
