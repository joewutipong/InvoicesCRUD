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
    private readonly IRunningNumbersRepository _runningNumbersRepository;
    private readonly Mock<IRunningNumbersRepository> _runningNumbersRepositoryMock;
    private readonly IInvoicesService _invoiceService;

    public InvoicesServiceTest()
    {
        _fixture = new Fixture();
        _invoicesRepositoryMock = new Mock<IInvoicesRepository>();
        _invoicesRepository = _invoicesRepositoryMock.Object;
        _runningNumbersRepositoryMock = new Mock<IRunningNumbersRepository>();
        _runningNumbersRepository = _runningNumbersRepositoryMock.Object;
        _invoiceService = new InvoicesService(_invoicesRepository, _runningNumbersRepository);
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
