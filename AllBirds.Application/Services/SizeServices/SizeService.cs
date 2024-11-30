using AllBirds.Application.Contracts;
using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SizeDTOs;
using AllBirds.Models;
using AutoMapper;
namespace AllBirds.Application.Services.SizeServices
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IMapper _mapper;

        public SizeService(ISizeRepository sizeRepository, IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _mapper = mapper;
        }

        public async Task<ResultView<CUSizeDTO>> CreateAsync(CUSizeDTO cUSizeDTO)
        {
            ResultView<CUSizeDTO> resultView = new();
            try
            {
                bool Check = (await _sizeRepository.GetAllAsync()).Any(P => P.Id == cUSizeDTO.Id || P.SizeNumber == cUSizeDTO.SizeNumber);
                if (Check)
                {
                    resultView.Data = cUSizeDTO;
                    resultView.IsSuccess = false;
                    resultView.Msg = $"This Size {cUSizeDTO.SizeNumber} Already Exist";
                    return resultView;
                }
                Size mappedSize = _mapper.Map<Size>(cUSizeDTO);
                Size createdSize = await _sizeRepository.CreateAsync(mappedSize);
                if (createdSize is not null)
                {
                    await _sizeRepository.SaveChangesAsync();
                    CUSizeDTO cUSize = _mapper.Map<CUSizeDTO>(createdSize);
                    resultView.Data = cUSize;
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Size {cUSize.SizeNumber} Created Successfully";
                    return resultView;
                }


                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Size {cUSizeDTO.SizeNumber} Not Created Successfully";
                return resultView;


            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Size ${cUSizeDTO.SizeNumber} ${ex.Message}";
            }
            return resultView;

        }

        public async Task<ResultView<CUSizeDTO>> UpdateAsync(CUSizeDTO cUSizeDTO)
        {
            ResultView<CUSizeDTO> resultView = new();
            try
            {
                bool Check = (await _sizeRepository.GetAllAsync()).Any(P => P.Id != cUSizeDTO.Id && P.SizeNumber == cUSizeDTO.SizeNumber);
                if (Check)
                {
                    resultView.Data = cUSizeDTO;
                    resultView.IsSuccess = false;
                    resultView.Msg = $"Size {cUSizeDTO.SizeNumber} Already Exist";
                    return resultView;
                }
                Size? sizeObj = (await _sizeRepository.GetAllAsync()).FirstOrDefault(s => s.Id == cUSizeDTO.Id && s.IsDeleted == false);
                if (sizeObj is not null)
                {
                    sizeObj.SizeNumber = cUSizeDTO.SizeNumber;
                    await _sizeRepository.SaveChangesAsync();
                    CUSizeDTO cUSize = _mapper.Map<CUSizeDTO>(sizeObj);
                    resultView.Data = cUSize;
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Size {cUSize.SizeNumber} Updated Successfully";
                    return resultView;
                }
                resultView.Data = null;
                resultView.IsSuccess = false;
                resultView.Msg = $"Size {cUSizeDTO.SizeNumber} Not Updated Successfully";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Updated Size ${cUSizeDTO.SizeNumber} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<CUSizeDTO> SoftDeleteAsync(int sizeId)
        {
            Size? sizeObj = (await _sizeRepository.GetAllAsync()).FirstOrDefault(s => s.Id == sizeId && s.IsDeleted == false);
            if (sizeObj is not null)
            {
                sizeObj.IsDeleted = true;
                await _sizeRepository.SaveChangesAsync();
                return _mapper.Map<CUSizeDTO>(sizeObj);
            }
            return null;
        }
        public async Task<ResultView<CUSizeDTO>> HardDeleteAsync(int sizeId) // will this throw tracking exception ??
        {
            ResultView<CUSizeDTO> resultView = new();
            try
            {
                bool dependentPrds = (await _sizeRepository.GetAllAsync()).Any(s => s.Id == sizeId && s.Products.Count > 0);
                if (dependentPrds)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = "Cannot Delete This Size As There Are Product Color Variants That Depend On It";
                    return resultView;
                }
                Size? sizeObj = (await _sizeRepository.GetAllAsync()).FirstOrDefault(s => s.Id == sizeId && s.IsDeleted == false);
                if (sizeObj is not null)
                {
                    Size deletedSize = await _sizeRepository.DeleteAsync(sizeObj);

                    await _sizeRepository.SaveChangesAsync();
                    CUSizeDTO cUSize = _mapper.Map<CUSizeDTO>(deletedSize);
                    resultView.IsSuccess = true;
                    resultView.Data = cUSize;
                    resultView.Msg = $"Size {cUSize.SizeNumber} Deleted Successfully ";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Size Is Not Found ";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Deleted Size ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<List<CUSizeDTO>>> GetAllAsync()
        {
            ResultView<List<CUSizeDTO>> resultView = new();
            try
            {
                List<Size> sizesList = [.. (await _sizeRepository.GetAllAsync()).Where(s => !s.IsDeleted)];
                if (sizesList.Count > 0)
                {
                    List<CUSizeDTO> cUSizes = _mapper.Map<List<CUSizeDTO>>(sizesList);
                    resultView.IsSuccess = true;
                    resultView.Data = cUSizes;
                    resultView.Msg = "All Size Fetched Successfully";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "Sizes Is Empty..! Sorry";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Deleted Size ${ex.Message}";
            }
            return resultView;
        }

        public async Task<List<CUSizeDTO>> GetAllWithDeletedAsync()
        {
            List<Size> sizesList = [.. (await _sizeRepository.GetAllAsync())];
            return _mapper.Map<List<CUSizeDTO>>(sizesList);
        }
        public async Task<GetSizeDTO> GetByIdAsync(int sizeId)
        {
            var sizeList = await _sizeRepository.GetAllAsync();
            var sizeObj = sizeList.FirstOrDefault(s => s.Id == sizeId && !s.IsDeleted); // bool operator

            if (sizeObj != null)
            {
                return _mapper.Map<GetSizeDTO>(sizeObj);
            }
            return null;
        }
    }
}
