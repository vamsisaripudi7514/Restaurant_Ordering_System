async function getAnalytics() {
    const response = await fetch('http://localhost:5222/owner/get-analytics');
    const data = await response.json();
    console.log(data);
    const pending_orders_element = document.getElementById("pending-orders");
    const total_revenue_element = document.getElementById("total-revenue");
    const total_transactions_element = document.getElementById("total-transactions");
    const total_customers_element = document.getElementById("total-customers");
    const top_selling_item_element = document.getElementById("top-selling-item");
    const least_selling_item_element = document.getElementById("least-selling-item");
    pending_orders_element.innerText = `${data.pendingOrdersCount}`;
    total_revenue_element.innerText = `${data.totalRevenue}$`;
    total_transactions_element.innerText = `${data.totalTransactions}`;
    total_customers_element.innerText = `${data.customerCount}`;
    top_selling_item_element.innerText = `${data.topSellingItem.item_Name}`;
    least_selling_item_element.innerText = `${data.leastSellingItem}`;
}
window.onload = function () {
    getAnalytics();

};
