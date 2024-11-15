
async function GetOrders() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    console.log(Customer_ID);
    if (Customer_ID == null) {
        Customer_ID = 'CUST001';
    }
    console.log(Customer_ID);
    const response = await fetch(`http://localhost:5222/customer/get-orders/${Customer_ID}`);
    const tableBody = document.querySelector('#customer-orders-table tbody');
    tableBody.innerHTML = ''; 
    if (response.ok) {
        var pendingOrders = await response.json();
        console.log(pendingOrders);
        if (Array.isArray(pendingOrders) && pendingOrders.length > 0) {
            pendingOrders.forEach(order => {
                const row = document.createElement('tr');
                row.innerHTML = `
                
                <td>${order.item_Name}</td>
                <td>${order.quantity}</td>
                <td>${order.order_Value}</td>
                <td>${order.status ? 'Served' : 'Pending'}</td>
               
            `;
                tableBody.appendChild(row);
            });
        }
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No order history found.</td>`;
        tableBody.appendChild(row);
    }
}

window.onload = function () {
    GetOrders();
}