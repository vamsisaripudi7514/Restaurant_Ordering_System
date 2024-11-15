async function getMenu() {
    var response = await fetch("http://localhost:5222/owner/get-menu");
    var data = await response.json();
    console.log(data);
    const tableBody = document.querySelector('#menu-table tbody');
    tableBody.innerHTML = ''; 

    if (Array.isArray(data) && data.length > 0) {
        data.forEach(menu => {
            const row = document.createElement('tr');
                  
            row.innerHTML = `
                
                <td>${menu.item_Name}</td>
                <td>${menu.price}</td>
                <td>${menu.availability}</td>
                <td>
                    <button type="button" class="btn btn-block bg-gradient-olive" style="width:70%;" onclick="updateAvailability('${menu.item_ID}')">Change Status</button>
                </td>
                
            `;
            
            tableBody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4">No menu items found.</td>`;
        tableBody.appendChild(row);
    }
}
async function updateAvailability(itemId) {
    try {
        await fetch(`http://localhost:5222/owner/update-menu/${itemId}`, {
            method: 'PUT', 
        });
    } catch (error) {
        console.error('Failed to update availability', error);
    }
    getMenu();
}
window.onload = function () {
    getMenu();
}
