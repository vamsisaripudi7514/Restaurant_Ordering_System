
async function fetchPendingPayments() {
    const response = await fetch('http://localhost:5222/owner/get-pending-payments');
    const tableBody = document.querySelector('#pending-payments-table tbody');
    tableBody.innerHTML = '';
    if (response.ok) {
        const pendingPayments = await response.json();
        console.log(pendingPayments);

        if (Array.isArray(pendingPayments) && pendingPayments.length > 0) {
            pendingPayments.forEach(payment => {
                const row = document.createElement('tr');
                row.innerHTML = `
                <td>${payment.customer_Name}</td>
                <td>${payment.customer_Mail}</td>
                <td>${payment.total_Amount}</td>
                <td>
                    <button type="button" class="btn btn-block bg-gradient-success" onclick="markPayment('${payment.payment_ID}')">Mark as Paid</button>
                </td>
            `;
                tableBody.appendChild(row);
            });
        }
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No pending payments found.</td>`;
        tableBody.appendChild(row);
    }

    document.getElementById('paid-payments').style.display = 'none';
    document.getElementById('pending-payments').style.display = 'block';
}
async function fetchPaidPayments() {
    const response = await fetch('http://localhost:5222/owner/get-paid-payments');
    const paidPayments = await response.json();
    const tableBody = document.querySelector('#paid-payments-table tbody');
    tableBody.innerHTML = ''; 
    console.log(paidPayments);
    if (Array.isArray(paidPayments) && paidPayments.length > 0) {
        paidPayments.forEach(payment => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${payment.customer_Name}</td>
                <td>${payment.customer_Mail}</td>
                <td>${payment.total_Amount}</td>
                <td>${payment.payment_Status ? 'Paid' : 'Pending'}</td>
            `;
            tableBody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No paid payments found.</td>`;
        tableBody.appendChild(row);
    }

    document.getElementById('pending-payments').style.display = 'none';
    document.getElementById('paid-payments').style.display = 'block';
}
async function markPayment(paymentId) {
    try {
        const response = await fetch(`http://localhost:5222/owner/mark-payment/${paymentId}`, {
            method: 'PUT'
        });

        if (response.ok) {
            const data = await response.json();
            Swal.fire({
                icon: 'success',
                title: `Payment Marked as Successful`,
                toast: true,
                position: 'top-end', 
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });

            fetchPendingPayments(); 
        } else {
            const data = await response.text();
            Swal.fire({
                icon: 'error',
                title: 'Failed to mark the payment as paid.',
                text: data,
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
    } catch (error) {
        console.error("Error marking payment as paid:", error);
        Swal.fire({
            icon: 'error',
            title: 'An error occurred.',
            text: error.message,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }
}
window.onload = function () {
    fetchPendingPayments();
};