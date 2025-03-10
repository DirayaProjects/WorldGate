function initializeDataTable(tableSelector) {
    $(tableSelector).dataTable({
        dom: 'Bfrtip',
        lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
        buttons: [
            {
                extend: 'copy',
                className: 'custom-button',
                exportOptions: {
                    columns: ':not(:last-child)'
                },
                text: '<i class="fas fa-copy"></i> Copy'
            },
            {
                extend: 'csv',
                className: 'custom-button',
                exportOptions: {
                    columns: ':not(:last-child)'
                },
                text: '<i class="fas fa-file-csv"></i> CSV'
            },
            {
                extend: 'excel',
                className: 'custom-button',
                exportOptions: {
                    columns: ':not(:last-child)'
                },
                text: '<i class="fas fa-file-excel"></i> Excel'
            },
            {
                extend: 'pdf',
                className: 'custom-button',
                exportOptions: {
                    columns: ':not(:last-child)'
                },
                text: '<i class="fas fa-file-pdf"></i> PDF'
            },
            {
                extend: 'print',
                className: 'custom-button',
                exportOptions: {
                    columns: ':not(:last-child)'
                },
                text: '<i class="fas fa-print"></i> Print'
            },
            'pageLength'
        ]
    });
}


function initializeDataTableWithoutPermission(tableSelector) {
    $(tableSelector).dataTable({
        dom: 'Bfrtip',
        buttons: ['pageLength'],
        lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]]
    });
}
