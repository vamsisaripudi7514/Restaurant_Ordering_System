async function GetMenuItems() {
    var Customer_ID = await sessionStorage.getItem('Customer_ID');
    var Cart_ID = await sessionStorage.getItem('Cart_ID');
    console.log(Customer_ID); console.log(Cart_ID);
    var response = await fetch('http://localhost:5222/customer/get-menu');
    const tableBody = document.querySelector('#customer-menu-table tbody');
    tableBody.innerHTML = ''; 
    if (response.ok) {
        var data = await response.json();
        console.log(data);
        if (Array.isArray(data) && data.length > 0) {
            data.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
                 <td><a href="#" onclick="showPopup('${item.item_Name}', '${item.description}')"><b>${item.item_Name}</b></a></td>
                <td>${item.price}</td>
                <td>
                    <input type="number" id="${item.item_ID}" min="1"  value=1 style="width: 40px;">
                </td>
                <td>
                    <button class="btn btn-block bg-gradient-success" style="width:50%;" onclick="AddToCart('${item.item_ID}')">ADD TO CART</button>
                </td>
                `;
                tableBody.appendChild(row);
                });

        }
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No item history found.</td>`;
        tableBody.appendChild(row);
    }
}

function showPopup(title, description) {
    console.log(title,description);
    document.getElementById('popupTitle').textContent = title;
    document.getElementById('popupDescription').textContent = description;
    document.getElementById('itemDescriptionPopup').style.display = 'block';
}

function closePopup() {
    document.getElementById('itemDescriptionPopup').style.display = 'none';
}
async function AddToCart(item_ID) {
    var Customer_ID = await sessionStorage.getItem('Customer_ID');
    var Cart_ID = await sessionStorage.getItem('Cart_ID');
    console.log(Customer_ID); console.log(Cart_ID);
    const response = await fetch('http://localhost:5222/customer/add-to-cart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            Cart_ID: Cart_ID,
            Customer_ID: Customer_ID,
            Item_ID: item_ID,
            Quantity: document.getElementById(`${item_ID}`).value
        })
    });
    if (response.ok) {
        Swal.fire({
            icon: 'success',
            title: `The Item is added to Cart.`,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });

    } else {
        Swal.fire({
            icon: 'error',
            title: 'Failed to add Item Cart.',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        console.log(await response.text());
    }
}
window.onload = function () {
    GetMenuItems();
}