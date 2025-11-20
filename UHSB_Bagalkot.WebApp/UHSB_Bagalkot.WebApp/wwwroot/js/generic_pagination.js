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
    let end = start + pg.rowsPerPage;

    let paginatedData = pg.data.slice(start, end);

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

    let html = `
        <li class="page-item ${pg.currentPage === 1 ? "disabled" : ""}">
            <a class="page-link" href="#" onclick="loadPage('${gridId}', ${pg.currentPage - 1})">Previous</a>
        </li>
    `;

    for (let i = 1; i <= totalPages; i++) {
        html += `
            <li class="page-item ${pg.currentPage === i ? "active" : ""}">
                <a class="page-link" href="#" onclick="loadPage('${gridId}', ${i})">${i}</a>
            </li>
        `;
    }

    html += `
        <li class="page-item ${pg.currentPage === totalPages ? "disabled" : ""}">
            <a class="page-link" href="#" onclick="loadPage('${gridId}', ${pg.currentPage + 1})">Next</a>
        </li>
    `;

    container.html(html);
}
