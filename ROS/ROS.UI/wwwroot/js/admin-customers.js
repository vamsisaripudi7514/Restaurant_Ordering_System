async function getCustomers() {
    const response = await fetch("http://localhost:5222/owner/get-customers");
    const tableBody = document.querySelector('#customers-table tbody');
    tableBody.innerHTML = '';
    if (response.ok) {
        const data = await response.json();
        console.log(data);

        if (Array.isArray(data) && data.length > 0) {
            data.forEach(data   => {
                const row = document.createElement('tr');
                row.innerHTML = `
                <td>${data.customer_Name}</td>
                <td>${data.customer_Mail}</td>
                <td>${data.customer_Phone}</td>
                `;
                tableBody.appendChild(row);
            });
        }
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No customers found.</td>`;
        tableBody.appendChild(row);
    }

}

window.onload = function () {
    getCustomers();
}