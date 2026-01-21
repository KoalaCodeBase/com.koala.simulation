document.addEventListener("DOMContentLoaded", () => {
  const tds = document.querySelectorAll(".unity-table td:nth-child(3)");
  console.log("Found description cells:", tds.length);

  tds.forEach(td => {
    td.style.cursor = "pointer";
    td.addEventListener("click", () => {
      console.log("Clicked:", td.textContent.trim()); // debug
      const fullText = td.textContent.trim();

      const overlay = document.createElement("div");
      overlay.className = "popup-overlay";

      const box = document.createElement("div");
      box.className = "popup-box";
      box.textContent = fullText;

      overlay.appendChild(box);
      document.body.appendChild(overlay);

      overlay.addEventListener("click", () => overlay.remove());
    });
  });
});