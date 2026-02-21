using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IProductServices
    {
        Task<int> Add_Product(ProductController.AddProductRequest request);
        Task<int> UpdateProduct(ProductController.UpdateProductRequest request);
        Task<int> DeleteProduct(long id);
        Task<List<ProductController.Product_list>> Get_product_list();
        Task<ProductController.SingleProductList?> Get_product_by_id(long id);
        Task<List<ProductController.Drop_Product_List>> Get_drop_productlist();
        Task<int> Add_ProductDetail(ProductController.Add_ProductDetail_Request request);
        Task<int> Update_ProductDetail(ProductController.Update_ProductDetail_Request request);
        Task<int> Delete_ProductDetail(long id);
        Task<List<ProductController.ProductDetail_List>> Get_ProductDetail_List();
        //Task<ProductController.Single_ProductDetail?> Get_ProductDetail_By_Id(long id);
        Task<List<ProductController.Single_ProductDetail>> Get_ProductDetail_By_Id(long id);
        Task<List<ProductController.Drop_ProductDetail>> Get_Drop_ProductDetailList(long Comp_id, long Fin_year_id);

    }

}




