const API_URL = "https://localhost:44383";

import { Router } from "./Javascripts/router/router.js";
import { loadLogin } from "./Javascripts/login/loadLogin.js";
import { loadRegister } from "./Javascripts/register/loadRegister.js";
import { loadLists } from "./Javascripts/lists/loadLists.js";
import { loadListDetail } from "./Javascripts/LoadListDetail/loadListDetail.js";
import { logout } from "./Javascripts/login/loadLogin.js";

document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token");
  const logoutButton = document.getElementById("btnLogout");

  if (token) {
    logoutButton.style.display = "block";
  } else {
    logoutButton.style.display = "none";
  }

  logoutButton.addEventListener("click", () => {
    logout();
  });
});

const main = document.querySelector("main");
getAllRoutes();

export function getAllRoutes() {
  const routes = {
    login: { hash: "#login", function: loadLogin },
    register: { hash: "#register", function: loadRegister },
    lists: { hash: "#lists", function: loadLists },
    listDetail: { hash: "#listDetail", function: loadListDetail },
    error: { function: renderNotFound },
  };

  const router = new Router(routes);
  router.urlResolve();

  function renderNotFound() {
    main.innerHTML = `
  <h1>404 | Not found</h1>
  <a class="flexbox btn notFound" href="#">Return to login</a>`;
  }
}
