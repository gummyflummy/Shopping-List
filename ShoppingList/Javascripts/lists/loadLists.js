const API_URL = 'https://localhost:44383';

export async function loadLists() {
     styleHolder.innerHTML =
            '<link rel="stylesheet" href="./Stylesheets/listsStyle.css"></link>'
    const main = document.querySelector('main'); 
    const listsPage = `
        <div class="header">
            <h1>Your Lists</h1>
            <button id="btnAddList">Add New List</button>
        </div>
        <div class="listsContainer">
            <ul id="listsUl"></ul>
        </div>
    `;
    
    main.innerHTML = listsPage;

    const listsUl = document.getElementById('listsUl');

    try {
        const response = await fetch(`${API_URL}/api/lists`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error('Failed to load lists');
        }

        const lists = await response.json();


        lists.forEach((list) => {
            const createdDate = new Date(list.createdDate); 
            const formattedDate = createdDate.toLocaleDateString(); 
            
            const listItem = document.createElement('li');
            listItem.className = 'listItem';
            listItem.innerHTML = `
                <span>List ID: ${list.listID}</span>
                <span>Created: ${formattedDate}</span> 
                <button class="btnOpen" data-id="${list.listID}">open</button>
                <button class="btnDelete" data-id="${list.listID}">delete</button>
            `;
            listsUl.appendChild(listItem);
        });


        document.querySelectorAll('.btnOpen').forEach(button => {
            button.addEventListener('click', (event) => {
                const listID = event.target.getAttribute('data-id');
                openList(listID);
            });
        });

        document.querySelectorAll('.btnDelete').forEach(button => {
            button.addEventListener('click', (event) => {
                const listID = event.target.getAttribute('data-id');
                console.log(`Deleting list with ID: ${listID}`); 
                deleteList(listID);
            });
        });
        document.getElementById('btnAddList').addEventListener('click', addList);
    } catch (error) {
        console.error(error);
    }
}

async function addList() {
    const token = localStorage.getItem('token');
    const userId = getUserIdFromToken(token);
    const currentDate = new Date().toISOString(); 

    try {
        const response = await fetch(`${API_URL}/api/lists`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({
                userID: userId,
                createdDate: currentDate 
            }), 
        });

        if (!response.ok) {
            throw new Error('Failed to add list');
        }

        loadLists(); 
    } catch (error) {
        console.error(error);
    }
}


function getUserIdFromToken(token) {
    if (!token) {
        console.error("No token found.");
        return null;
    }
    const payload = token.split('.')[1];
    const decodedPayload = atob(payload);
    const payloadJson = JSON.parse(decodedPayload);
    return payloadJson.userId;
}

async function openList(listID) {
    location.hash = `#list?id=${listID}`;
}
async function deleteList(listID) {
    try {
        const response = await fetch(`${API_URL}/api/lists/${listID}`, {
            method: 'DELETE',
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`,
            },
        });

        if (!response.ok) {
            throw new Error('Failed to delete list');
        }

        loadLists(); 
    } catch (error) {
        console.error(error);
    }
}
