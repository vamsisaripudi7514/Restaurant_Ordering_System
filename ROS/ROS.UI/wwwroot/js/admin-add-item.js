async function createItem() {
    const item_Name = document.getElementById('Item_Name').value;
    const item_Description = document.getElementById('Description').value;
    const item_Price = document.getElementById('Price').value;

    console.log(item_Name);
    console.log(item_Description);
    console.log(item_Price);

    const response = await fetch('http://localhost:5222/owner/add-item', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            Item_ID: "hello",
            Item_Name: item_Name,
            Description: item_Description,
            Price: item_Price,
            Availability: true
        })
    });

    if (response.ok) {
        alert(`Item Added.`);
        window.location.href = '../../admin/menu';
    } else {
        const errorData = await response.text();
        alert(errorData);
    }
}

document.getElementById('item-info-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    await createItem();
});