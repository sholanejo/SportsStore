﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{
    public class HomeControllerTests : Controller
    {
        [Fact]
        public void Can_Use_Repository()
        {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},
                new Product{ProductID = 2, Name = "P2"}
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object);

            //ACT
            ProductsListViewModel result = controller.Index(null).ViewData.Model as ProductsListViewModel;


            //ASSERT
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},
                new Product{ProductID = 2, Name = "P2"},
                new Product{ProductID = 3, Name = "P3"},
                new Product{ProductID = 4, Name = "P4"},
                new Product{ProductID = 5, Name = "P5"},
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            //ACT
            ProductsListViewModel result = controller.Index(null, 2).ViewData.Model as ProductsListViewModel;


            //ASSERT
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
             new Product {ProductID = 1, Name = "P1"},
             new Product {ProductID = 2, Name = "P2"},
             new Product {ProductID = 3, Name = "P3"},
             new Product {ProductID = 4, Name = "P4"},
             new Product {ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());
            // Arrange
            HomeController controller = new HomeController(mock.Object) { PageSize = 3 };
            // Act
            ProductsListViewModel result = controller.Index(null, 2).ViewData.Model as ProductsListViewModel;
            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Products()
        {
            // Arrange
            // - create the mock repository
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                 new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                 new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                 new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                 new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                 new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                 }).AsQueryable<Product>());
            // Arrange - create a controller and make the page size 3 items
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;
            // Action
            Product[] result =
            (controller.Index("Cat2", 1).ViewData.Model as ProductsListViewModel)
            .Products.ToArray();
            // Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
    }
}
