const tileSize = 85;
const bounds = [[0, 0],[-256 * tileSize, 256 * tileSize]];

const map = L.map('map', {
    crs: L.CRS.Simple
}).setView([127.5 * tileSize, 127.5 * tileSize], 0);

const tile = L.tileLayer('image/{x}.{y}.png', {
    tileSize: tileSize,
    bounds: bounds,
    tms: true,
    continuousWorld: true,
    maxNativeZoom: 0
}).addTo(map);

map.setMaxBounds(bounds); // Définissez les limites maximales de la carte