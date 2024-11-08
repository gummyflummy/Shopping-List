const API_URL = "https://localhost:44383";

export async function loadLogin() {
  styleHolder.innerHTML =
    '<link rel="stylesheet" href="./Stylesheets/loginStyle.css"></link>';
  main.innerHTML = login;
  document.querySelector("#btnLogin").addEventListener("click", loginUser);
  document.querySelector("#inputUsername").addEventListener("keydown", (e) => {
    if (e.key === "Enter") document.querySelector("#inputPassword").focus();
  });
  document.querySelector("#inputPassword").addEventListener("keydown", (e) => {
    if (e.key === "Enter") loginUser();
  });
}
let errorMessage;
const logoutButton = document.getElementById("btnLogout");
const styleHolder = document.getElementById("styleHolder");
const main = document.querySelector("main");

const login = `<div class="flexbox mainHeader">
<a id="switchToRegister" href="#register">New here? Register.</a>
</div>
<p class="siteInfo">Log-in</p>
<div class="flexbox inputDivLogin">
<input id="inputUsername" class="inputLogin" type="text" placeholder="Username">
<input id="inputPassword" class="inputLogin" type="password" placeholder="Password">
<p class="errorMessage">Username or Password wrong</p>
<div class="flexbox loginBtnDiv">
    <a class="flexbox btn" id="btnLogin">Continue</a>
</div>
</div>`;

async function loginUser() {
  const username = document.getElementById("inputUsername").value;
  const password = document.getElementById("inputPassword").value;

  try {
    const response = await fetch(`${API_URL}/api/auth/login`, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    });

    const data = await response.json();
    if (!response.ok) {
      throw new Error("Login failed");
    }

    localStorage.setItem("token", data);
    alert("Login successful");
    logoutButton.style.display = "block";
    window.location.hash = "#lists";
  } catch (error) {
    console.error(error);
    errorMessage.innerHTML = "Login failed, please try again.";
    errorMessage.style.display = "block";
  }
}

export async function logout() {
  const token = localStorage.getItem("token");
  if (!token) {
    alert("You're already logged out.");
    return;
  }

  try {
    const response = await fetch(`${API_URL}/api/auth/logout`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) throw new Error("Failed to logout");

    alert("Logout successful");
    localStorage.removeItem("token");
    logoutButton.style.display = "none";
    window.location.href = "#login";
  } catch (error) {
    console.error("Error logging out:", error);
    alert("Error: " + error.message);
  }
}
