/* ---------------- Generic Pagination System (Multi-Grid Support) ---------------- */ 

let paginationRegistry = {};

function initPagination(gridId, data, renderFunction, rowsPerPage = 5) {
    paginationRegistry[gridId] = {
        data,
        renderFunction,
        rowsPerPage,
        currentPage: 1
    };
    loadPage(gridId, 1);
}

function loadPage(gridId, page) {
    let pg = paginationRegistry[gridId];
    pg.currentPage = page;

    let start = (page - 1) * pg.rowsPerPage;
    let paginatedData = pg.data.slice(start, start + pg.rowsPerPage);

    pg.renderFunction(paginatedData);
    renderPagination(gridId);
}

function renderPagination(gridId) {
    let pg = paginationRegistry[gridId];
    let totalPages = Math.ceil(pg.data.length / pg.rowsPerPage);
    let container = $("#" + gridId + "PaginationContainer");

    if (totalPages <= 1) {
        container.html("");
        return;
    }

    let html = "";

    // First
    html += `
        <li class="page-item ${pg.currentPage === 1 ? "disabled" : ""}">
            <a class="page-link" onclick="loadPage('${gridId}', 1)">⏮</a>
        </li>
    `;

    // Prev
    html += `
        <li class="page-item ${pg.currentPage === 1 ? "disabled" : ""}">
            <a class="page-link" onclick="loadPage('${gridId}', ${pg.currentPage - 1})">&laquo;</a>
        </li>
    `;

    // Show 5 numbers logic
    let maxButtons = 5;
    let start = pg.currentPage - Math.floor(maxButtons / 2);
    let end = pg.currentPage + Math.floor(maxButtons / 2);

    if (start < 1) {
        end += (1 - start);
        start = 1;
    }
    if (end > totalPages) {
        start -= (end - totalPages);
        end = totalPages;
    }
    if (start < 1) start = 1;

    for (let i = start; i <= end; i++) {
        html += `
            <li class="page-item ${pg.currentPage === i ? "active" : ""}">
                <a class="page-link" onclick="loadPage('${gridId}', ${i})">${i}</a>
            </li>
        `;
    }

    // Next
    html += `
        <li class="page-item ${pg.currentPage === totalPages ? "disabled" : ""}">
            <a class="page-link" onclick="loadPage('${gridId}', ${pg.currentPage + 1})">&raquo;</a>
        </li>
    `;

    // Last
    html += `
        <li class="page-item ${pg.currentPage === totalPages ? "disabled" : ""}">
            <a class="page-link" onclick="loadPage('${gridId}', ${totalPages})">⏭</a>
        </li>
    `;

    container.html(html);
}

