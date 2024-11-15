async function createCustomer() {
    const customer_Name = document.getElementById('Customer_Name').value;
    const customer_Phone = document.getElementById('Customer_Phone').value;
    const customer_Mail = document.getElementById('Customer_Mail').value;

    console.log(customer_Name);
    console.log(customer_Phone);
    console.log(customer_Mail);

    const response = await fetch('http://localhost:5222/customer/create-customer', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            Customer_ID: "hello",
            Customer_Name: customer_Name,
            Customer_Mail: customer_Mail,
            Customer_Phone: customer_Phone
        })
    });
    var Customer_ID = 'CUST001';
    sessionStorage.setItem('Customer_ID', Customer_ID);
    var Cart_ID = 'CART001';
    sessionStorage.setItem('Cart_ID', Cart_ID);
    console.log(sessionStorage.Customer_ID);
    console.log(sessionStorage.Cart_ID);
    if (response.ok) {
        data  = await response.json();
        sessionStorage.setItem('Customer_ID', data.customer_ID);
        sessionStorage.setItem('Cart_ID', data.cart_ID);
        console.log(sessionStorage.Customer_ID);
        console.log(sessionStorage.Cart_ID);
        if (data.message == null)
            alert(`Customer Added.`);
        else
            alert("Welcome Back!!");
        window.location.href = '../../customer';
    } else {
        const errorData = await response.text();
        alert(errorData);
        console.log(errorData);
        //sessionStorage.setItem('Customer_ID', customer_ID);//check-------------------------------------
        if (errorData = "The customer already exists") {
            window.location.href = '../../customer';
        }
    }
}

document.getElementById('user-info-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    await createCustomer();
});


