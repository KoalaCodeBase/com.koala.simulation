export default {
    start: () => {
        console.log("custom main.js loaded ✅");

        const wordLimit = 25; // tablo için kelime limiti

        document.querySelectorAll(".unity-table tbody tr").forEach(tr => {
            tr.style.cursor = "pointer";

            const cells = tr.querySelectorAll("td");
            if (cells.length < 3) return;

            const descCell = cells[2];
            const fullHtml = descCell.innerHTML.trim();
            const fullText = descCell.textContent.trim();
            const words = fullText.split(/\s+/);

            // description kısalt
            if (words.length > wordLimit) {
                descCell.setAttribute("data-full", fullHtml);
                descCell.textContent = words.slice(0, wordLimit).join(" ") + "…";
            } else {
                descCell.setAttribute("data-full", fullHtml);
            }

            // popup click
            tr.addEventListener("click", () => {
                const name = cells[0].textContent.trim();
                const declaration = cells[1].textContent.trim();
                const description = descCell.getAttribute("data-full");

                // overlay
                const overlay = document.createElement("div");
                overlay.className = "popup-overlay";

                // popup kutusu
                const box = document.createElement("div");
                box.className = "popup-box";

                // içerik layout
                box.innerHTML = `
          <div class="popup-content">
            <div class="label">Name</div>
            <div class="value">${name}</div>

            <div class="label">Declaration</div>
            <div class="value"><code>${declaration}</code></div>

            <div class="label">Description</div>
            <div class="value description">${description}</div>
          </div>
        `;

                overlay.appendChild(box);
                document.body.appendChild(overlay);

                // sadece overlay'e tıklayınca kapanacak
                overlay.addEventListener("click", (e) => {
                    if (e.target === overlay) overlay.remove();
                });
            });
        });
    },
};