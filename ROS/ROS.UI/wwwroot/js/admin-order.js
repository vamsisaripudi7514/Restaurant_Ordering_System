async function fetchPendingOrders() {
    const response = await fetch('http://localhost:5222/chef/pending-orders');
    var pendingOrders = await response.json();
    console.log(pendingOrders);
    const tableBody = document.querySelector('#pending-orders-table tbody');
    tableBody.innerHTML = ''; 

    if (Array.isArray(pendingOrders) && pendingOrders.length > 0) {
        pendingOrders.forEach(order => {
            const row = document.createElement('tr');
            row.innerHTML = `
                
                <td>${order.customerName}</td>
                <td>${order.itemName}</td>
                <td>${order.quantity}</td>
                <td>${order.price}</td>
               
            `;
            tableBody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No pending orders found.</td>`;
        tableBody.appendChild(row);
    }

    document.getElementById('served-orders').style.display = 'none';
    document.getElementById('pending-orders').style.display = 'block';
}

async function fetchServedOrders() {
    const response = await fetch('http://localhost:5222/chef/served-orders');
    const servedOrders = await response.json();
    console.log(servedOrders);
    const tableBody = document.querySelector('#served-orders-table tbody');
    tableBody.innerHTML = ''; 

    if (Array.isArray(servedOrders) && servedOrders.length > 0) {
        servedOrders.forEach(order => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${order.customerName}</td>
                <td>${order.itemName}</td>
                <td>${order.quantity}</td>
                <td>${order.price}</td>
            `;
            tableBody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="3">No served orders found.</td>`;
        tableBody.appendChild(row);
    }

    document.getElementById('pending-orders').style.display = 'none';
    document.getElementById('served-orders').style.display = 'block';
}
window.onload = function () {
    fetchPendingOrders();
};