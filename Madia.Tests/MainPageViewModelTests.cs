using System;
using System.Threading.Tasks;
using Madia.Services;
using Madia.ViewModels;
using Xunit;

namespace Madia.Tests;

public class MainPageViewModelTests
{
    [Fact]
    public async Task ProcessBarcodeAsync_WhenProductFound_UpdatesUiState()
    {
        var vm = new MainPageViewModel(new FakeProductLookupService
        {
            Result = new ProductLookupResult
            {
                IsSuccess = true,
                IsFound = true,
                ProductName = "Milk",
                ProductBrand = "Local Farm",
                ImageUrl = "https://example.com/milk.png"
            }
        });

        await vm.ProcessBarcodeAsync("123456");

        Assert.Equal("Scanned: 123456", vm.ResultText);
        Assert.Equal("Milk", vm.ProductName);
        Assert.Equal("Local Farm", vm.ProductBrand);
        Assert.Equal("https://example.com/milk.png", vm.ProductImageUrl);
    }

    [Fact]
    public async Task ProcessBarcodeAsync_WhenProductNotFound_SetsNotFoundState()
    {
        var vm = new MainPageViewModel(new FakeProductLookupService
        {
            Result = new ProductLookupResult
            {
                IsSuccess = true,
                IsFound = false
            }
        });

        await vm.ProcessBarcodeAsync("000000");

        Assert.Equal("Scanned: 000000", vm.ResultText);
        Assert.Equal("Product not found", vm.ProductName);
        Assert.Equal(string.Empty, vm.ProductBrand);
        Assert.Null(vm.ProductImageUrl);
    }

    [Fact]
    public async Task ProcessBarcodeAsync_WhenLookupFails_SetsErrorState()
    {
        var vm = new MainPageViewModel(new FakeProductLookupService
        {
            Result = new ProductLookupResult
            {
                IsSuccess = false,
                ErrorMessage = "network down"
            }
        });

        await vm.ProcessBarcodeAsync("111111");

        Assert.Equal("Error: network down", vm.ProductName);
        Assert.Equal(string.Empty, vm.ProductBrand);
        Assert.Null(vm.ProductImageUrl);
    }

    [Fact]
    public void RestartCommand_WhenExecuted_ResetsStateAndRaisesEvent()
    {
        var vm = new MainPageViewModel(new FakeProductLookupService
        {
            Result = new ProductLookupResult
            {
                IsSuccess = true,
                IsFound = true,
                ProductName = "Bread",
                ProductBrand = "Bakery",
                ImageUrl = "https://example.com/bread.png"
            }
        });

        var raised = false;
        vm.RestartRequested += () => raised = true;

        vm.ResultText = "Scanned: 123";
        vm.ProductName = "Bread";
        vm.ProductBrand = "Bakery";
        vm.RestartCommand.Execute(null);

        Assert.True(raised);
        Assert.Equal("Point camera at a barcode", vm.ResultText);
        Assert.Equal(string.Empty, vm.ProductName);
        Assert.Equal(string.Empty, vm.ProductBrand);
        Assert.Null(vm.ProductImageUrl);
    }

    sealed class FakeProductLookupService : IProductLookupService
    {
        public ProductLookupResult Result { get; set; } = new ProductLookupResult
        {
            IsSuccess = true,
            IsFound = false
        };

        public Task<ProductLookupResult> LookupByBarcodeAsync(string barcode)
        {
            return Task.FromResult(Result);
        }
    }
}
