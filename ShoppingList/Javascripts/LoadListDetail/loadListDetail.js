const API_URL = "https://localhost:44383";

async function loadProducts(listID, productsUl) {
  try {
    const response = await fetch(`${API_URL}/api/shoppinglist`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) throw new Error("Failed to load shopping list items");
    const shoppingLists = await response.json();
    const filteredProducts = shoppingLists.filter(
      (sl) => String(sl.listID) === String(listID)
    );

    for (const sl of filteredProducts) {
      try {
        const productResponse = await fetch(
          `${API_URL}/api/products/${sl.productID}`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
              "Content-Type": "application/json",
            },
          }
        );

        if (!productResponse.ok)
          throw new Error(
            `Failed to load details for productID: ${sl.productID}`
          );

        const productDetails = await productResponse.json();

        const productItem = document.createElement("li");
        productItem.className = "productItem";
        productItem.innerHTML = `
                    <span>${productDetails.name || "Unnamed"} - ${
          productDetails.quantity || "N/A"
        } ${productDetails.unit || "N/A"}</span>
                    <button class="openDetailsBtn" data-product-id="${
                      productDetails.productID
                    }">View</button>
                    <button class="deleteProductBtn" data-product-id="${
                      productDetails.productID
                    }">Delete</button>
                `;
        productsUl.appendChild(productItem);

        const openDetailsBtn = productItem.querySelector(".openDetailsBtn");
        openDetailsBtn.addEventListener("click", async () => {
          const productID = openDetailsBtn.getAttribute("data-product-id");
          openProductDetails(productID);
        });

        const deleteProductBtn = productItem.querySelector(".deleteProductBtn");
        deleteProductBtn.addEventListener("click", async () => {
          await deleteProduct(productDetails.productID, listID, productsUl);
        });
      } catch (error) {
        console.error(`Error loading product ${sl.productID}:`, error);
      }
    }
  } catch (error) {
    console.error("Error loading shopping list:", error);
  }
}

async function deleteProduct(productID, listID) {
  try {
    const response = await fetch(`${API_URL}/api/products/${productID}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) throw new Error(`Failed to delete product ${productID}`);

    alert("Product deleted successfully");
    document.getElementById("productsUl").innerHTML = "";
    await loadProducts(listID, document.getElementById("productsUl"));
  } catch (error) {
    console.error("Error deleting product:", error);
    alert("Error: " + error.message);
  }
}

async function openProductDetails(productID) {
  try {
    const response = await fetch(`${API_URL}/api/products/${productID}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok)
      throw new Error(
        `Failed to load product details for product ${productID}`
      );

    const productDetails = await response.json();
    showProductDetails(productDetails);
  } catch (error) {
    console.error(error);
    alert(error.message);
  }
}

async function showProductDetails(product) {
  if (
    !product ||
    !product.name ||
    !product.description ||
    !product.price ||
    !product.quantity ||
    !product.imagePath
  ) {
    console.error("Missing product details:", product);
    alert("Product details are missing");
    return;
  }

  const productNameElem = document.getElementById("productName");
  const productDescriptionElem = document.getElementById("productDescription");
  const productPriceElem = document.getElementById("productPrice");
  const productQuantityElem = document.getElementById("productQuantity");
  const productImageElem = document.getElementById("productImage");
  const imagePath = `${API_URL}${product.imagePath}`;
  productImageElem.src = imagePath;

  productNameElem.textContent = product.name || "No Name";
  productDescriptionElem.textContent = product.description || "No Description";
  productPriceElem.textContent = `${product.price || "0.00"}`;
  productQuantityElem.textContent = `${product.quantity || 0} ${
    product.unit || "N/A"
  }`;

  if (product.imagePath) {
    productImageElem.src = `${API_URL}${product.imagePath}`;
    productImageElem.alt = product.name;
  } else {
    productImageElem.src = "";
    productImageElem.alt = "No picture available";
  }

  const detailPopup = document.getElementById("productDetailPopup");
  detailPopup.style.display = "block";

  const closeButton = detailPopup.querySelector(".close-button");
  closeButton.addEventListener("click", closePopup);

  window.onclick = function (event) {
    if (event.target === detailPopup) {
      closePopup();
    }
  };
}

async function closePopup() {
  const popup = document.getElementById("productDetailPopup");
  if (popup) {
    popup.style.display = "none";
  }
  window.onclick = null;
}

async function openProductPopup(listID) {
  const popup = document.getElementById("addProductPopup");
  const closePopupButton = document.getElementById("closePopup");
  const addProductForm = document.getElementById("addProductForm");

  popup.style.display = "block";

  closePopupButton.onclick = () => {
    popup.style.display = "none";
  };

  window.onclick = (event) => {
    if (event.target === popup) {
      popup.style.display = "none";
    }
  };

  addProductForm.onsubmit = async (event) => {
    event.preventDefault();

    const AddproductName = document.getElementById("AddproductName").value;
    const AddproductDescription = document.getElementById(
      "AddproductDescription"
    ).value;
    const AddproductPrice = document.getElementById("AddproductPrice").value;
    const AddproductQuantity =
      document.getElementById("AddproductQuantity").value;
    const AddproductUnit = document.getElementById("AddproductUnit").value;
    const productImage = document.getElementById("AddproductImage").files[0];

    try {
      const formData = new FormData();
      formData.append("file", productImage);

      const imageResponse = await fetch(
        `${API_URL}/api/products/upload-image`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: formData,
        }
      );
      //test
      if (!imageResponse.ok) throw new Error("Failed to upload image");

      const { imagePath } = await imageResponse.json();

      const productResponse = await fetch(`${API_URL}/api/products`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name: AddproductName,
          description: AddproductDescription,
          price: AddproductPrice,
          quantity: AddproductQuantity,
          unit: AddproductUnit,
          imagePath,
        }),
      });

      if (!productResponse.ok) throw new Error("Failed to add product");

      const productData = await productResponse.json();

      const listResponse = await fetch(`${API_URL}/api/shoppinglist`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          productID: productData.productID,
          listID: listID,
        }),
      });

      if (!listResponse.ok)
        throw new Error("Failed to add product to shopping list");

      alert("Product added successfully");
      popup.style.display = "none";
      document.getElementById("productsUl").innerHTML = "";
      await loadProducts(listID, document.getElementById("productsUl"));
    } catch (error) {
      console.error(error);
      alert(error.message);
    }
  };
}
export async function loadListDetail(listID) {
  document.getElementById("styleHolder").innerHTML =
    '<link rel="stylesheet" href="./Stylesheets/listDetailStyle.css"></link>';

  const main = document.querySelector("main");

  const listDetailPage = `
        <div class="listHeader">
            <h1>List Details (ID: ${listID})</h1>
            <button id="btnAddProduct">Add Product</button>
        </div>
        <div class="productsContainer">
            <ul id="productsUl"></ul>
        </div>
        <div id="addProductPopup" class="popup">
            <div class="popup-content">
                <span class="close" id="closePopup">&times;</span>
                <h2>Add Product</h2>
                <form id="addProductForm" enctype="multipart/form-data">
                    <label for="productName">Product Name:</label>
                    <input type="text" id="AddproductName" required />
                    <label for="productDescription">Description:</label>
                    <input type="text" id="AddproductDescription" required />
                    <label for="productPrice">Price:</label>
                    <input type="text" id="AddproductPrice" required />
                    <label for="productQuantity">Quantity:</label>
                    <input type="number" id="AddproductQuantity" required />
                    <label for="productUnit">Unit:</label>
                    <input type="text" id="AddproductUnit" required />
                    <label for="productImage">Upload Image:</label>
                    <input type="file" id="AddproductImage" accept="image/*" />
                    <button type="submit">Add Product</button>
                </form>
            </div>
        </div>
        <div id="productDetailPopup" class="product-detail-popup" style="display: none;">
            <div class="popup-content">
                <span class="close-button">&times;</span>
                <h2 id="productName"></h2>
                <img id="productImage" src="" alt="Product Image" />
                <p><strong>Description:</strong> <span id="productDescription"></span></p>
                <p><strong>Price:</strong> <span id="productPrice"></span></p>
                <p><strong>Quantity:</strong> <span id="productQuantity"></span></p>
            </div>
        </div>
    `;
  main.innerHTML = listDetailPage;

  const productsUl = document.getElementById("productsUl");
  await loadProducts(listID, productsUl);

  document.getElementById("btnAddProduct").addEventListener("click", () => {
    openProductPopup(listID);
  });
}
