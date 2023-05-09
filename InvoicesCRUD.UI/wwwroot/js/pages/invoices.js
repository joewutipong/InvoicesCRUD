function formatCurrencyDisplay(value) {
    let USDollar = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    });

    return USDollar.format(value)
}

function confirmDeleteInvoice(invoiceId, invoiceNumber) {
    $('#DeleteInvoiceNumber').text(invoiceNumber);
    $('#InvoiceIdToDelete').val(invoiceId)
}

function checkAll(checkbox) {
    $('input:checkbox').prop('checked', checkbox.checked);
}

function displayAlert(message) {
    $('#AlertMessage').text(message);
    $('#Alert').fadeIn('slow');

    setTimeout(() => {
        $('#Alert').fadeOut('slow');
    }, 5000);
}

function hideAlert() {
    $('#Alert').fadeOut('slow');
}

function exportExcel() {
    var itemsCount = $('input[type="checkbox"][name="SelectedInvoice"]:checked').length;

    if (itemsCount == 0) {
        displayAlert('Please select invoices for export.');
        return;
    }

    $('#ExportExcelForm').submit();
}