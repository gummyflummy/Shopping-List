const API_URL = "https://localhost:44383";

export function loadRegister() {
  styleHolder.innerHTML =
    '<link rel="stylesheet" href="./Stylesheets/loginStyle.css"></link>';
  main.innerHTML = register;
  errorMessage = document.querySelector(".errorMessage");
  document.querySelector("#btnLogin").addEventListener("click", registerUser);
  document.querySelector("#inputUsername").addEventListener("keydown", (e) => {
    if (e.key === "Enter") document.querySelector("#inputPassword").focus();
  });
  document.querySelector("#inputPassword").addEventListener("keydown", (e) => {
    if (e.key === "Enter")
      document.querySelector("#inputPasswordReenter").focus();
  });
  document
    .querySelector("#inputPasswordReenter")
    .addEventListener("keydown", (e) => {
      if (e.key === "Enter") registerUser();
    });
}

let errorMessage;
const styleHolder = document.getElementById("styleHolder");
const main = document.querySelector("main");
const register = `<div class="flexbox mainHeader">
<a id="switchToRegister" href="#login">Already registered? Log-in.</a>
</div>
<p class="siteInfo">Register</p>
<div class="flexbox inputDivLogin">
<input id="inputUsername" class="inputLogin" type="text" placeholder="Username">
<input id="inputPassword" class="inputLogin" type="password" placeholder="Password">
<input id="inputPasswordReenter" class="inputLogin" type="password" placeholder="Password-Confirmation">
<p class="errorMessage">Passwords are not identical</p>
<div class="flexbox loginBtnDiv">
    <a class="flexbox btn" id="btnLogin">Register</a>
</div>
</div>`;

async function registerUser() {
  const username = document.getElementById("inputUsername").value;
  const password = document.getElementById("inputPassword").value;
  const passwordReenter = document.getElementById("inputPasswordReenter").value;
  if (password != passwordReenter) {
    errorMessage.innerHTML = "Passwords are not identical";
    errorMessage.style.display = "block";
    return;
  } else {
    errorMessage.style.display = "none";
  }

  try {
    const response = await fetch(`${API_URL}/api/auth/register`, {
      method: "POST",
      headers: {
        Accept: "*/*",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    });

    if (!response.ok) {
      throw new Error("Registration failed");
    }

    alert("successfully registered");
    window.location.hash = "#login";
  } catch (error) {
    console.error(error);
    errorMessage.innerHTML = "Registration failed, please try again.";
    errorMessage.style.display = "block";
  }
}
