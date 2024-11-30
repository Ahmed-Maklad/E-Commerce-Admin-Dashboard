using AutoMapper;
using AllBirds.Models;
using AllBirds.DTOs.AccountDTOs;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.CategoryProductDTOs;
using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.CouponDTOs;
using AllBirds.DTOs.OrderDetailsDTOs;
using AllBirds.DTOs.OrderMasterDTOs;
using AllBirds.DTOs.OrderStateDTOs;
using AllBirds.DTOs.ProductColorDTOs;
using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.ProductSpecificationDTOs;
using AllBirds.DTOs.SizeDTOs;
using AllBirds.DTOs.SpecificationDTOs;
using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
namespace AllBirds.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Account
            CreateMap<CUAccountDTO, CustomUser>().ReverseMap();
            CreateMap<ClientRegisterDTO, CustomUser>().ReverseMap();
            CreateMap<AccountLoginDTO, CustomUser>().ReverseMap();
            CreateMap<CustomUser, GetAllAdminsDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? "NA"))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? "NA"))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? "NA"))
                .ReverseMap();
            CreateMap<CustomUser, ClientDetailsDTO>()
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.PostalCode));
            #endregion

            #region Category
            CreateMap<CUCategoryDTO, Category>().ReverseMap();
            CreateMap<GetAllCategoryDTO, Category>().ReverseMap();
            CreateMap<GetOneCategoryDTO, Category>().ReverseMap();
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            #endregion

            #region CategoryProduct
            //CreateMap<CreateOrUpdateBookAuthorDTO, BookAuthor>().ReverseMap();
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            //CreateMap<GetOneBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));
            #endregion

            #region ClientFavorite
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            #endregion

            #region Color
            CreateMap<CUColorDTO, Color>().ReverseMap();
            CreateMap<GetColorDTO, Color>().ReverseMap();
            #endregion

            #region Coupon
            CreateMap<CUCouponDTO, Coupon>().ReverseMap();
            CreateMap<GetCouponDTO, Coupon>().ReverseMap();
            #endregion

            #region OrderDetail
            CreateMap<GetAllOrderDetailsDTO, OrderDetail>().ReverseMap()
                .ForMember(dest=>dest.OrderMasterNo,opt=>opt.MapFrom(src=>src.OrderMaster.OrderNo));

            CreateMap<GetOneOrderDetailsDTO, OrderDetail>().ReverseMap();

            CreateMap<CreateOrderDetailDTO, OrderDetail>()
                .ForMember(dest => dest.ProductColorSizeId, opt => opt.MapFrom(src => src.ProductId));

            CreateMap<OrderDetail, CreateOrderDetailDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductColorSizeId));

            // CreateMap<OrderDetail,ProductColorSizeImageDTO>()
            //     .ForMember(dest => dest.ColorNameAR, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Color.NameAr))
            //     .ForMember(dest => dest.ColorNameEN, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Color.NameEn))
            //     .ForMember(dest => dest.ProductNameAR, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Product.NameAr))
            //     .ForMember(dest => dest.ProductNameEN, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Product.NameEn))
            //     .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ProductColorSize.Size.SizeNumber))
            //     .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Images.FirstOrDefault().ImagePath));

            //CreateMap<OrderDetail, GetAllCartCheckoutDetailsDTO>()
            //    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Product.NameEn))
            //    .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Color.NameEn))
            //    .ForMember(dest => dest.SizeNumber, opt => opt.MapFrom(src => src.ProductColorSize.Size.SizeNumber))
            //    .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Images
            //    .FirstOrDefault(i => i.Id == src.ProductColorSize.ProductColor.MainImageId).ImagePath));

            CreateMap<OrderDetail, ProductColorSizeImageDTO>()
                .ForMember(dest => dest.ColorNameAR, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Color.NameAr))
                .ForMember(dest => dest.ColorNameEN, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Color.NameEn))
                .ForMember(dest => dest.ProductNameAR, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Product.NameAr))
                .ForMember(dest => dest.ProductNameEN, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Product.NameEn))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ProductColorSize.Size.SizeNumber))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.ProductColorSize.ProductColor.Images.FirstOrDefault().ImagePath))
                ;
            #endregion

            #region OrderMaster
            CreateMap<GetAllOrderMastersDTO, OrderMaster>().ReverseMap()
                  .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Client.FirstName) ? "NA" : $"{src.Client.FirstName} {src.Client.LastName}"))
                .ForMember(dest => dest.OrderStateName, opt => opt.MapFrom(src => src.OrderState.StateEn))
                .ForMember(dest => dest.DiscountPerctnage, opt => opt.MapFrom(src => src.Coupon == null ? "0%" : $"{src.Coupon.Discount} %"))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Coupon.Discount * src.Total));

            CreateMap<CreateOrderMasterDTO, OrderMaster>()
                .ForMember(dest => dest.OrderNo, opt => opt.MapFrom(src => $"OM-{src.ClientId}{(int)src.Total}{string.Join("", src.ProductColorSizeId.Select(p => $"{p.ProductId}{p.Quantity}"))}{DateTime.Now.ToShortDateString()}"));
            //.ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.ProductColorSizeId));

            CreateMap<OrderMaster, CreateOrderMasterDTO>()
                .ForMember(dest => dest.ProductColorSizeId, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<GetOneOrderMasterDTO, OrderMaster>().ReverseMap()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => $"{src.Client.FirstName} {src.Client.LastName}"))
                .ForMember(dest => dest.OrderStateName, opt => opt.MapFrom(src => src.OrderState.StateEn))
                .ForMember(dest => dest.DiscountPerctnage, opt => opt.MapFrom(src => $"{src.Coupon.Discount} %"))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Coupon.Discount * src.Total));

            CreateMap<OrderMaster, GetUserCartCheckoutDTO>()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon.Code));
                //.ForMember(dest => dest.OrderStateNameEn, opt => opt.MapFrom(src => src.OrderState.StateEn))
                //.ForMember(dest => dest.OrderStateNameAr, opt => opt.MapFrom(src => src.OrderState.StateAr));
            #endregion

            #region OrderState
            CreateMap<CUOrderStateDTO, OrderState>().ReverseMap();
            CreateMap<GetOrderStateDTO, OrderState>().ReverseMap();
            #endregion

            #region Product
            CreateMap<Product, GetAllProductDTO>()
                .ForMember(dest => dest.MainImagePath, opt => opt.MapFrom(src =>
                src.AvailableColors.FirstOrDefault(pc => pc.Id == src.MainColorId).Images.FirstOrDefault(pci => pci.Id == src.AvailableColors.FirstOrDefault(pc => pc.Id == src.MainColorId).MainImageId).ImagePath))
                .ForMember(dest => dest.MainColorCode, opt => opt.MapFrom(src =>
                src.AvailableColors.FirstOrDefault(pc => pc.Id == src.MainColorId).Color.Code));
            CreateMap<CUProductDTO, Product>()
                .ForMember(dest => dest.HighlightsAr, opt => opt.MapFrom(src => JoinStringList(src.HighlightsAr)))
                .ForMember(dest => dest.HighlightsEn, opt => opt.MapFrom(src => JoinStringList(src.HighlightsEn)))
                .ForMember(dest => dest.SustainableMaterialsAr, opt => opt.MapFrom(src => JoinStringList(src.SustainableMaterialsAr)))
                .ForMember(dest => dest.SustainableMaterialsEn, opt => opt.MapFrom(src => JoinStringList(src.SustainableMaterialsEn)))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoriesId.Select(c => new CategoryProduct() { CategoryId = c, ProductId = src.Id })));

            CreateMap<Product, CUProductDTO>()
                .ForMember(dest => dest.HighlightsAr, opt => opt.MapFrom(src => SplitJoinedString(src.HighlightsAr)))
                .ForMember(dest => dest.HighlightsEn, opt => opt.MapFrom(src => SplitJoinedString(src.HighlightsEn)))
                .ForMember(dest => dest.SustainableMaterialsAr, opt => opt.MapFrom(src => SplitJoinedString(src.SustainableMaterialsAr)))
                .ForMember(dest => dest.SustainableMaterialsEn, opt => opt.MapFrom(src => SplitJoinedString(src.SustainableMaterialsEn)))
                .ForMember(dest => dest.CategoriesId, opt => opt.MapFrom(src => src.Categories.Select(cp => cp.CategoryId).ToList()));
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            #endregion

            #region ProductDetails
            CreateMap<ProductDetail, CRProductDetails>().ForMember(dest => dest.ImageData, opt => opt.Ignore()).ReverseMap();
            CreateMap<ProductDetail, UpdateProductDetail>().ForMember(dest => dest.ImageData, opt => opt.Ignore()).ReverseMap();
            CreateMap<ProductDetail, GetAllProductDetailsDTOS>().ForMember(dest => dest.ProductNo, opt => opt.MapFrom(src => src.Product.ProductNo))
                .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.Product.NameEn))
                .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.Product.NameAr));

            #endregion

            #region ProductColor

            CreateMap<ProductColor, CreateProductColorDTO>()
                        .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<CreateProductColorDTO, ProductColor>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<UpdateProductColorDTO, ProductColor>().ReverseMap();
            CreateMap<GetALlProductColorDTO, ProductColor>().ReverseMap()
                .ForMember(dest => dest.ColorNameAr, opt => opt.MapFrom(src => src.Color.NameAr))
                .ForMember(dest => dest.ColorNameEn, opt => opt.MapFrom(src => src.Color.NameEn))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src => src.Color.Code))
                .ForMember(dest => dest.PNameAr, opt => opt.MapFrom(src => src.Product.NameAr))
                .ForMember(dest => dest.PNameEn, opt => opt.MapFrom(src => src.Product.NameEn))
                .ForMember(dest => dest.ProductNo, opt => opt.MapFrom(src => src.Product.ProductNo))
                .ForMember(dest => dest.MainImagePath, opt => opt.MapFrom(src => src.Images.FirstOrDefault(p => p.Id == src.MainImageId).ImagePath));
            CreateMap<GetOneProductColorDTO, ProductColor>().ReverseMap()
                .ForMember(dest => dest.ColorNameAr, opt => opt.MapFrom(src => src.Color.NameAr))
                .ForMember(dest => dest.ColorNameEn, opt => opt.MapFrom(src => src.Color.NameEn))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src => src.Color.Code))
                .ForMember(dest => dest.PNameAr, opt => opt.MapFrom(src => src.Product.NameAr))
                .ForMember(dest => dest.PNameEn, opt => opt.MapFrom(src => src.Product.NameEn))
                .ForMember(dest => dest.ProductNo, opt => opt.MapFrom(src => src.Product.ProductNo))
                .ForMember(dest => dest.MainImagePath, opt => opt.MapFrom(src => src.Images.FirstOrDefault(p => p.Id == src.MainImageId).ImagePath))
                .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src => src.AvailableSizes.Select(r => r.Size.SizeNumber).ToList()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(r => r.ImagePath).ToList()))
                .ForMember(dest => dest.ProductColorImageId, opt => opt.MapFrom(src => src.Images.Select(r => r.Id).ToList()));




                            // CreateMap<Product, ProductCardDTO>()
                            //.ForMember(dest => dest.ProductColors, opt => opt.MapFrom(src => src.AvailableColors.Where(prdId => prdId.ProductId == src.Id).SelectMany(ac => ac.Images).Select(Img => Img.ImagePath)))
                            //.ForMember(dest => dest.PrdouctSizes, opt => opt.MapFrom(src => src.AvailableColors.Where(color => color.ProductId == src.Id).SelectMany(color => color.AvailableSizes.Where(size => size.ProductColorId == color.Id).Select(size => size.Size.SizeNumber))));

            #endregion

            #region ProductColorImage
            CreateMap<ProductColorImage, CUProductColorImageDTO>().ForMember(dest => dest.ImageData, opt => opt.Ignore()).ReverseMap();
            CreateMap<ProductColorImage, GetAllCategoryProductDTO>().ReverseMap();
            #endregion

            #region ProductColorSize
            CreateMap<CreatePCSDTO, ProductColorSize>();
            CreateMap<ProductColorSize, CreatePCSDTO>();
            CreateMap<UpdatePCSDTO, ProductColorSize>();
            CreateMap<ProductColorSize, UpdatePCSDTO>();
            CreateMap<ProductColorSize, GetPCSDTO>()
                .ForMember(dest => dest.ProductColorSizeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SizeNumber, opt => opt.MapFrom(src => src.Size.SizeNumber));
            #endregion

            #region ProductDetail
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            #endregion

            #region ProductReview
            //CreateMap<GetAllBookAuthorDTO, BookAuthor>().ReverseMap()
            //    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            #endregion

            #region ProductSpecification
            CreateMap<CUProductSpecificationDTO, ProductSpecification>().ReverseMap();
            CreateMap<ProductSpecification, GetProductSpecificationDTO>()
                .ForMember(dest => dest.ProductNo, opt => opt.MapFrom(src => src.Product.ProductNo))
                .ForMember(dest => dest.ProductNameAr, opt => opt.MapFrom(src => src.Product.NameAr))
                .ForMember(dest => dest.ProductNameEn, opt => opt.MapFrom(src => src.Product.NameEn))
                .ForMember(dest => dest.SpecificationNameAr, opt => opt.MapFrom(src => src.Specification.NameAr))
                .ForMember(dest => dest.SpecificationNameEn, opt => opt.MapFrom(src => src.Specification.NameEn));
            #endregion

            #region Size
            CreateMap<CUSizeDTO, Size>().ReverseMap();
            CreateMap<GetSizeDTO, Size>().ReverseMap();
            #endregion

            #region Specification
            CreateMap<CUSpecificationDTO, Specification>().ReverseMap();
            CreateMap<GetSpecificationDTO, Specification>().ReverseMap();
            #endregion

            //CreateMap<AdminAccRegisterDTOs, IdentityUser>()
            //.ForMember(dest => dest.Id, opt => opt.Ignore()) 
            //.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //.ReverseMap();
            //CreateMap<GetAccountAdminDTOs , IdentityUser>().ReverseMap();
            //CreateMap<LoginUserDTOs , IdentityUser>().ReverseMap();

        }
        private static string? JoinStringList(List<string>? list)
        {
            if (list == null)
                return null;

            return list.Count > 1
                ? String.Join("~@#$%&", list)
                : list.FirstOrDefault();
        }
        private static List<string>? SplitJoinedString(string? str) => str is not null ? str.Split("~@#$%&", StringSplitOptions.RemoveEmptyEntries).ToList() : null;
    }
}
