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

public class InvoicesServiceTest
{
    private readonly IFixture _fixture;
    private readonly IInvoicesRepository _invoicesRepository;
    private readonly Mock<IInvoicesRepository> _invoicesRepositoryMock;
    private readonly IInvoiceProductsRepository _invoiceProductsRepository;
    private readonly Mock<IInvoiceProductsRepository> _invoiceProductsRepositoryMock;
    private readonly IRunningNumbersRepository _runningNumbersRepository;
    private readonly Mock<IRunningNumbersRepository> _runningNumbersRepositoryMock;
    private readonly IInvoicesService _invoiceService;

    public InvoicesServiceTest()
    {
        _fixture = new Fixture();
        _invoicesRepositoryMock = new Mock<IInvoicesRepository>();
        _invoicesRepository = _invoicesRepositoryMock.Object;
        _invoiceProductsRepositoryMock = new Mock<IInvoiceProductsRepository>();
        _invoiceProductsRepository = _invoiceProductsRepositoryMock.Object;
        _runningNumbersRepositoryMock = new Mock<IRunningNumbersRepository>();
        _runningNumbersRepository = _runningNumbersRepositoryMock.Object;
        _invoiceService = new InvoicesService(_invoicesRepository, _invoiceProductsRepository, _runningNumbersRepository);
    }

    #region AddInvoice
    [Fact]
    public async Task AddInvoice_InvoiceAddRequestIsNull_ThrowArgumentNullExpectionAsync()
    {
        InvoiceAddRequest? invoiceAddRequest = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoice(invoiceAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddInvoice_DueDateIsNull_ThrowArgumentExceptionAsync()
    {
        InvoiceAddRequest? invoiceAddRequest = _fixture.Create<InvoiceAddRequest>();
        invoiceAddRequest.DueDate = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoice(invoiceAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoice_DueDateIsOlderThanToday_ThrowArgumentExceptionAsync()
    {
        InvoiceAddRequest? invoiceAddRequest = _fixture.Build<InvoiceAddRequest>()
            .With(temp => temp.DueDate, new DateTime(2000, 1, 1))
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoice(invoiceAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoice_CustomerNameIsNull_ThrowArgumentExceptionAsync()
    {
        InvoiceAddRequest? invoiceAddRequest = _fixture.Build<InvoiceAddRequest>()
            .With(temp => temp.CustomerName, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoice(invoiceAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoice_CustomerAddressIsNull_ThrowArgumentExceptionAsync()
    {
        InvoiceAddRequest? invoiceAddRequest = _fixture.Build<InvoiceAddRequest>()
            .With(temp => temp.CustomerAddress, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoice(invoiceAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddInvoice_WithValidInvoiceDetails_ShouldBeSuccessful()
    {
        InvoiceAddRequest addRequest = _fixture.Build<InvoiceAddRequest>()
            .With(temp => temp.DueDate, DateTime.Today)
            .Create();

        RunningNumber runningNumber = _fixture.Create<RunningNumber>();

        Invoice invoice = addRequest.ToInvoice();
        invoice.InvoiceNumber = "IV00001";

        InvoiceResponse invoiceExpected = invoice.ToInvoiceResponse();

        _runningNumbersRepositoryMock.Setup(temp => temp.GetRunningNumber(It.IsAny<RunningNumberTypes>())).ReturnsAsync(runningNumber);

        _invoicesRepositoryMock.Setup(temp => temp.AddInvoice(It.IsAny<Invoice>())).ReturnsAsync(invoice);

        InvoiceResponse? invoiceResponse = await _invoiceService.AddInvoice(addRequest);

        invoiceResponse.Should().BeEquivalentTo(invoiceExpected);
    }
    #endregion

    #region GetInvoices
    [Fact]
    public async Task GetInvoices_ShouldBeEmptyList()
    {
        List<Invoice> invoices = new List<Invoice>();

        _invoicesRepositoryMock.Setup(temp => temp.GetAllInvoices()).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesResponse = await _invoiceService.GetInvoices();

        invoicesResponse.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInvoices_WithFewInvoices_ShouldBeSuccessful()
    {
        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetAllInvoices()).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices();

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }
    #endregion

    #region GetInvoices(InvoiceFilters)
    [Fact]
    public async Task GetInvoicesWithFilters_InvoiceFiltersIsNull_ShouldBeReturnAllInvoices()
    {
        InvoiceFilters? filters = null;

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetAllInvoices()).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices();

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_FilterByInvoiceNumber_ShouldBeSuccessful()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.InvoiceNumber, "IV000")
            .With(temp => temp.CustomerName, null as string)
            .Create();
        filters.FromDueDate = null;
        filters.ToDueDate = null;

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetFilteredInvoices(It.IsAny<InvoiceFilters>())).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices(filters);

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_FilterByCustomerName_ShouldBeSuccessful()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.InvoiceNumber, null as string)
            .With(temp => temp.CustomerName, "Smith")
            .Create();
        filters.FromDueDate = null;
        filters.ToDueDate = null;

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetFilteredInvoices(It.IsAny<InvoiceFilters>())).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices(filters);

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_FilterByFromDueDate_ShouldBeSuccessful()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.InvoiceNumber, null as string)
            .With(temp => temp.CustomerName, null as string)
            .With(temp => temp.FromDueDate, DateTime.Parse("2023-04-25"))
            .Create();
        filters.ToDueDate = null;

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetFilteredInvoices(It.IsAny<InvoiceFilters>())).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices(filters);

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_FilterByToDueDate_ShouldBeSuccessful()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.InvoiceNumber, null as string)
            .With(temp => temp.CustomerName, null as string)
            .With(temp => temp.ToDueDate, new DateTime(2023, 4, 25))
            .Create();
        filters.FromDueDate = null;

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetFilteredInvoices(It.IsAny<InvoiceFilters>())).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices(filters);

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_FilterByAllFilterDetail_ShouldBeSuccessful()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.FromDueDate, new DateTime(2023, 4, 10))
            .With(temp => temp.ToDueDate, new DateTime(2023, 4, 25))
            .Create();

        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<InvoiceResponse> invoicesExpected = invoices.Select(temp => temp.ToInvoiceResponse()).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetFilteredInvoices(It.IsAny<InvoiceFilters>())).ReturnsAsync(invoices);

        List<InvoiceResponse>? invoicesActual = await _invoiceService.GetInvoices(filters);

        invoicesActual.Should().BeEquivalentTo(invoicesExpected);
    }

    [Fact]
    public async Task GetInvoicesWithFilters_ToDueDateIsOlderThanFromDueDate_ShouldThrowArgumentException()
    {
        InvoiceFilters filters = _fixture.Build<InvoiceFilters>()
            .With(temp => temp.FromDueDate, new DateTime(2023, 4, 25))
            .With(temp => temp.ToDueDate, new DateTime(2023, 4, 10))
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.GetInvoices(filters);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetInvoiceByInvoiceId
    [Fact]
    public async Task GetInvoiceByInvoiceId_InvoiceIdIsNull_ThrowArgumentNullException()
    {
        Guid? invoiceId = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.GetInvoiceByInvoiceId(invoiceId);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetInvoiceByInvoiceId_InvoiceIdIsNotValid_ShouldReturnNull()
    {
        Guid? invoiceId = Guid.NewGuid();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoiceByInvoiceId(It.IsAny<Guid>())).ReturnsAsync(null as Invoice);

        InvoiceResponse? invoiceResponse = await _invoiceService.GetInvoiceByInvoiceId(invoiceId);

        invoiceResponse.Should().BeNull();
    }

    [Fact]
    public async Task GetInvoiceByInvoiceId_InvoiceIdIsValid_ShouldReturnNull()
    {
        Invoice invoice = _fixture.Create<Invoice>();

        InvoiceResponse invoiceExpected = invoice.ToInvoiceResponse();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoiceByInvoiceId(It.IsAny<Guid>())).ReturnsAsync(invoice);

        InvoiceResponse? invoiceResponse = await _invoiceService.GetInvoiceByInvoiceId(invoice.InvoiceId);

        invoiceResponse.Should().BeEquivalentTo(invoiceExpected);
    }

    #endregion

    #region UpdateInvoice
    [Fact]
    public async Task UpdateInvoice_InvoiceUpDateRequestIsNull_ThrowArgumentNullException()
    {
        InvoiceUpdateRequest? updateRequest = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.UpdateInvoice(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateInvoice_DueDateIsNull_ThrowArgumentException()
    {
        InvoiceUpdateRequest updateRequest = _fixture.Create<InvoiceUpdateRequest>();
        updateRequest.DueDate = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.UpdateInvoice(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoice_DueDateIsOlderThanToday_ThrowArgumentException()
    {
        InvoiceUpdateRequest updateRequest = _fixture.Build<InvoiceUpdateRequest>()
            .With(temp => temp.DueDate, new DateTime(2000, 1, 1))
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.UpdateInvoice(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoice_CustomerNameIsNull_ThrowArgumentException()
    {
        InvoiceUpdateRequest updateRequest = _fixture.Build<InvoiceUpdateRequest>()
            .With(temp => temp.CustomerName, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.UpdateInvoice(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoice_CustomerAddressIsNull_ThrowArgumentException()
    {
        InvoiceUpdateRequest updateRequest = _fixture.Build<InvoiceUpdateRequest>()
            .With(temp => temp.CustomerAddress, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await _invoiceService.UpdateInvoice(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateInvoice_WithValidInvoiceDetails_ShouldBeSuccessful()
    {
        InvoiceUpdateRequest? updateRequest = _fixture.Create<InvoiceUpdateRequest>();

        Invoice invoice = updateRequest.ToInvoice();

        InvoiceResponse invoiceExpected = invoice.ToInvoiceResponse();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoiceByInvoiceId(It.IsAny<Guid>())).ReturnsAsync(invoice);

        InvoiceResponse? invoiceResponse = await _invoiceService.UpdateInvoice(updateRequest);

        invoiceResponse.Should().BeEquivalentTo(invoiceExpected);
    }
    #endregion

    #region DeleteInvoiceByInvoiceId
    [Fact]
    public async Task DeleteInvoiceByInvoiceId_InvoiceIdIsNull_ShouldBeSuccessful()
    {
        Guid? invoiceId = null;

        bool isDeleted = await _invoiceService.DeleteInvoiceByInvoiceId(invoiceId);

        isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteInvoiceByInvoiceId_InvoiceIdIsNotNull_ShouldBeSuccessful()
    {
        Guid invoiceId = Guid.NewGuid();

        bool isDeleted = await _invoiceService.DeleteInvoiceByInvoiceId(invoiceId);

        isDeleted.Should().BeTrue();
    }
    #endregion

    #region GenerateInvoiceNumber
    [Fact]
    public async Task GenerateInvoiceNumber_RunningNumberIsNull_ThrowException()
    {
        RunningNumber? runningNumber = null;

        _runningNumbersRepositoryMock.Setup(temp => temp.GetRunningNumber(It.IsAny<RunningNumberTypes>())).ReturnsAsync(runningNumber);

        Func<Task> action = async () =>
        {
            await _invoiceService.GenerateInvoiceNumber();
        };

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GenerateInvoiceNumber_RunningNumberIsNotNull_ThrowException()
    {
        RunningNumber? runningNumber = _fixture.Build<RunningNumber>()
            .With(temp => temp.RunningNumberType, RunningNumberTypes.Invoice.ToString())
            .With(temp => temp.Prefix, "IV")
            .With(temp => temp.CurrentRunning, 0)
            .Create();

        _runningNumbersRepositoryMock.Setup(temp => temp.GetRunningNumber(It.IsAny<RunningNumberTypes>())).ReturnsAsync(runningNumber);

        string? invoiceNumber = await _invoiceService.GenerateInvoiceNumber();

        invoiceNumber.Should().Be("IV00001");
    }
    #endregion

    #region AddInvoiceProduct
    [Fact]
    public async Task AddInvoiceProduct_InvoiceProductAddRequestIsNull_ThrowArgumentNullException()
    {
        InvoiceProductAddRequest? addRequest = null;

        Func<Task> action = async () =>
        {
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.AddInvoiceProduct(addRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.UpdateInvoiceProduct(updateRequest);
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
            await _invoiceService.DeleteInvoiceProduct(invoiceProductId);
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
            await _invoiceService.DeleteInvoiceProduct(invoiceProductId);
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
            await _invoiceService.DeleteInvoiceProduct(invoiceProduct.InvoiceProductId);
        };

        await action.Should().NotThrowAsync();
    }
    #endregion

    #region GetInvoiceExcel
    [Fact]
    public async Task GetInvoiceExcel_ShouldBeSuccesssful()
    {
        List<Invoice> invoices = new List<Invoice>()
        {
            _fixture.Create<Invoice>(),
            _fixture.Create<Invoice>(),
        };

        List<Guid> invoiceIds = invoices.Select(temp => temp.InvoiceId).ToList();

        _invoicesRepositoryMock.Setup(temp => temp.GetInvoicesByInvoiceIds(It.IsAny<List<Guid>>())).ReturnsAsync(invoices);

        var result = await _invoiceService.GetInvoicesExcel(invoiceIds);

        result.Should().BeOfType<MemoryStream>();
    }
    #endregion
}
