const API_URL = "https://localhost:44383";

export class Router {
  constructor(routes) {
    this.routes = routes;
    this.navigate = function (hash) {
      let route = this.getRouteByHash(hash);
      history.pushState({}, "", hash);
      const id = route.id;
      route.function(id);
    };

    this.urlResolve = function () {
      const hash = location.hash || "#login";
      this.navigate(hash);
    };

    this.getRouteByHash = (hash) => {
      if (hash === "") {
        return this.routes["login"];
      }
      let route = this.routes["error"];
      Object.keys(this.routes).forEach((key) => {
        if (this.routes[key].hash === hash) {
          route = this.routes[key];
        } else if (hash.startsWith("#list?id=")) {
          route = {
            function: this.routes["listDetail"].function,
            id: hash.split("=")[1],
          };
        }
      });
      return route;
    };

    addEventListener("hashchange", () => {
      this.urlResolve();
    });
  }
}
