using AutoFixture;
using FluentAssertions;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.DTO;
using InvoicesCRUD.Core.Enums;
using InvoicesCRUD.Core.ServiceContracts;
using InvoicesCRUD.Core.Services;
using Microsoft.VisualBasic;
using Moq;

namespace ServiceTests;

public class InvoiceProductsServiceTest
{
    private readonly IFixture _fixture;
    private readonly IInvoicesRepository _invoicesRepository;
    private readonly Mock<IInvoicesRepository> _invoicesRepositoryMock;
    private readonly IInvoiceProductsRepository _invoiceProductsRepository;
    private readonly Mock<IInvoiceProductsRepository> _invoiceProductsRepositoryMock;
    private readonly IInvoiceProductsService _invoiceProductsService;

    public InvoiceProductsServiceTest()
    {
        _fixture = new Fixture();
        _invoicesRepositoryMock = new Mock<IInvoicesRepository>();
        _invoicesRepository = _invoicesRepositoryMock.Object;
        _invoiceProductsRepositoryMock = new Mock<IInvoiceProductsRepository>();
        _invoiceProductsRepository = _invoiceProductsRepositoryMock.Object;

        _invoiceProductsService = new InvoiceProductsService(_invoicesRepository, _invoiceProductsRepository);
    }

    #region AddInvoiceProduct
    [Fact]
    public async Task AddInvoiceProduct_InvoiceProductAddRequestIsNull_ThrowArgumentNullException()
    {
        InvoiceProductAddRequest? addRequest = null;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_InvoiceIdIsNull_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Build<InvoiceProductAddRequest>()
            .With(temp => temp.InvoiceId, Guid.Empty)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_ProductNameIsNull_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Build<InvoiceProductAddRequest>()
            .With(temp => temp.ProductName, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_ProductPriceIsLessThan1_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();
        addRequest.ProductPrice = 0;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_ProductPriceIsGreaterThan1000000_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();
        addRequest.ProductPrice = 1000001;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_ProductIsQuantityLessThan1_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();
        addRequest.Quantity = 0;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_ProductQuantityIsGreaterThan1000_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();
        addRequest.Quantity = 1001;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_InvoiceIdIsNotValid_ThrowArgumentException()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoiceByInvoiceId(It.IsAny<Guid>())).ReturnsAsync(null as Invoice);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoiceProduct_WithValidInvoiceProductDetail_ShouldBeSuccessful()
    {
        InvoiceProductAddRequest addRequest = _fixture.Create<InvoiceProductAddRequest>();

        InvoiceProduct invoiceProduct = addRequest.ToInvoiceProduct();

        Invoice invoice = _fixture.Create<Invoice>();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoiceByInvoiceId(It.IsAny<Guid>())).ReturnsAsync(invoice);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.AddInvoiceProduct(addRequest);
        };

        await action.Should().NotThrowAsync();
    }
    #endregion

    #region UpdateInvoiceProduc
    [Fact]
    public async Task UpdateInvoiceProduct_InvoiceProductUpdateRequestIsNull_ThrowArgumentNullException()
    {
        InvoiceProductUpdateRequest? updateRequest = null;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_InvoiceIdIsNull_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.InvoiceId, Guid.Empty)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_InvoiceProductIdIsNull_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.InvoiceProductId, Guid.Empty)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductNameIsNull_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.ProductName, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductPriceIsLessThan1_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.ProductPrice, 0)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductPriceIsGreaterThan1000000_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.ProductPrice, 1000001)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductQuantityIsLessThan1_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.Quantity, 0)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductQuantityIsGreaterThan1000_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Build<InvoiceProductUpdateRequest>()
            .With(temp => temp.Quantity, 1001)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_ProductIdIsNotValid_ThrowArgumentException()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Create<InvoiceProductUpdateRequest>();

        _invoiceProductsRepositoryMock.Setup(temp => temp.GetInvoiceProductById(It.IsAny<Guid>())).ReturnsAsync(null as InvoiceProduct);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoiceProduct_WithValidInvoiceProductDetail_ShouldBeSuccessful()
    {
        InvoiceProductUpdateRequest updateRequest = _fixture.Create<InvoiceProductUpdateRequest>();

        InvoiceProduct invoiceProduct = updateRequest.ToInvoiceProduct();

        _invoiceProductsRepositoryMock.Setup(temp => temp.GetInvoiceProductById(It.IsAny<Guid>())).ReturnsAsync(invoiceProduct);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.UpdateInvoiceProduct(updateRequest);
        };

        await action.Should().NotThrowAsync();
    }
    #endregion

    #region DeleteInvoiceProduct
    [Fact]
    public async Task DeleteInvoiceProduct_InvoiceProductIdIsNull_ThrowArgumentNullException()
    {
        Guid? invoiceProductId = null;

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.DeleteInvoiceProduct(invoiceProductId);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteInvoiceProduct_InvoiceProductIdIsNotValid_ThrowArgumentNullException()
    {
        Guid? invoiceProductId = Guid.NewGuid();

        _invoiceProductsRepositoryMock.Setup(temp => temp.GetInvoiceProductById(It.IsAny<Guid>())).ReturnsAsync(null as InvoiceProduct);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.DeleteInvoiceProduct(invoiceProductId);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteInvoiceProduct_InvoiceProductIdIsValid_ShouldBeSuccessful()
    {
        InvoiceProduct invoiceProduct = _fixture.Create<InvoiceProduct>();

        _invoiceProductsRepositoryMock.Setup(temp => temp.GetInvoiceProductById(It.IsAny<Guid>())).ReturnsAsync(invoiceProduct);

        Func<Task> action = async () =>
        {
            await _invoiceProductsService.DeleteInvoiceProduct(invoiceProduct.InvoiceProductId);
        };

        await action.Should().NotThrowAsync();
    }
    #endregion
}
