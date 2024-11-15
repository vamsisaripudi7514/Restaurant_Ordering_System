
async function GetCart() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    const response = await fetch(`http://localhost:5222/customer/cart-items/${Customer_ID}`);
    const tableBody = document.querySelector('#customer-cart-table tbody');
    if (tableBody == null) return;
    tableBody.innerHTML = '';
    var total_Cost = 0;
    if (response.ok) {
        var data = await response.json();
        console.log(data);
        if (Array.isArray(data) && data.length > 0) {
            var items_to_checkout = [];
            data.forEach(cart => {
                cart.items.forEach(item => { 
                const row = document.createElement('tr');
                row.innerHTML = `
                
                <td><b>${item.item_Name}</b></td>
                <td>${item.quantity}</td>
                <td>${item.quantity * item.price}</td>
                <td>
                    <button class="btn btn-block bg-gradient-success" style="width:50%;" onclick="RemoveFromCart('${item.item_ID}')">REMOVE</button>
                </td> 
               
                `;
                    tableBody.appendChild(row);
                    total_Cost += item.quantity * item.price;
                    items_to_checkout.push({ Item_ID: item.item_ID, Quantity: item.quantity });
                });
            });
            console.log(total_Cost);
            document.getElementById('total-amount').innerHTML = `${total_Cost}`;
            sessionStorage.setItem('Total_Cost', total_Cost);
            return items_to_checkout;
        }
    } else {
        const row = document.createElement('tr');
        document.getElementById('total-amount').innerHTML = `${total_Cost}`;
        row.innerHTML = `<td colspan="4">The cart is empty.</td>`;
        tableBody.appendChild(row);
    }
}

async function GetCartItems() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    const response = await fetch(`http://localhost:5222/customer/cart-items/${Customer_ID}`);
    var total_Cost = 0;
    if (response.ok) {
        var data = await response.json();
        console.log(data); var items_to_checkout = [];
        if (Array.isArray(data) && data.length > 0) {
            data.forEach(cart => {
                cart.items.forEach(item => {
                    total_Cost += item.quantity * item.price;
                    items_to_checkout.push({ Item_ID: item.item_ID, Quantity: item.quantity });
                });
            });
            console.log(total_Cost);
            sessionStorage.setItem('Total_Cost', total_Cost);
            return items_to_checkout;
        }
    } else {
        return items_to_checkout;
    }
}
async function RemoveFromCart(item_ID) {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    var Cart_ID = sessionStorage.getItem('Cart_ID');
    console.log(Customer_ID); console.log(Cart_ID); console.log(item_ID);
    const response = await fetch(`http://localhost:5222/customer/remove-from-cart/${Cart_ID}/${Customer_ID}/${item_ID}`,{
        method:'PUT'
    });
    if (response.ok) {
        Swal.fire({
            icon: 'success',
            title: `Item has been removed from the cart`,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        GetCart();
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Failed to remove item.',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }
}

async function PlaceOrder(){
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    var Cart_ID = sessionStorage.getItem('Cart_ID');
    console.log(Customer_ID); console.log(Cart_ID);
    var Items = await GetCartItems();
    var data = {
        Customer_ID: Customer_ID,
        Table_ID: 1,
        Items
    }
    const response = await fetch("http://localhost:5222/customer/place-order", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });
    if (response.ok) {
        const result = await response.text();
        //console.log(result);
        alert("Order placed successfully!");
        for (const item of Items) {
            await RemoveFromCart(item.Item_ID);
            console.log(item.Item_ID); 
        }
        GetCart();
    } else {
        const error = await response.text();
        console.error("Order failed:", error);
        alert("Failed to place order: error");
    }
    console.log(data);
}
async function CheckOut() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    const response = await fetch(`http://localhost:5222/customer/cart-items/${Customer_ID}`);
    if (!response.ok) { alert('No Cart Items Found.'); return; }
    const confirmation = confirm("Are you sure you want to place the order?");
    if (!confirmation) {
        alert("Order canceled.");
        return;
    }
    window.location.href = '../customer/payment-method-selection';
}
async function CashPayment() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    var Total_Cost = sessionStorage.getItem('Total_Cost');
    console.log(Total_Cost);
    const response = await fetch(`http://localhost:5222/customer/add-to-payments/${Customer_ID}/${Total_Cost}/false`, {
        method:'POST'
    });
    if (response.ok) {
        const res = await PlaceOrder();;
        window.location.href = '/customer/cart';
    }
    else {
        alert("Failed to register payment.");
    }
}
async function OnlinePaymentRedirect() {
    window.location.href = '/customer/dummy-payment'; 
}

async function OnlinePayment() {
    var Customer_ID = sessionStorage.getItem('Customer_ID');
    var Total_Cost = sessionStorage.getItem('Total_Cost');
    console.log(Total_Cost);
    const response = await fetch(`http://localhost:5222/customer/add-to-payments/${Customer_ID}/${Total_Cost}/true`, {
        method: 'POST'
    });
    if (response.ok) {
        const res = await PlaceOrder();;
        window.location.href = '/customer/cart';
    }
    else {
        alert("Payment Failed.");
    }
}
async function MakePayment(type) {
    if (type == 'online') {

    }
    else {

    }

}
window.onload = function () {
    GetCart();
}