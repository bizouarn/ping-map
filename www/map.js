const tileSize = 265;
const bounds = [[0, 0],[-14 * tileSize, 14 * tileSize]];

const map = L.map('map', {
    crs: L.CRS.Simple
}).setView([0, 0], 0);

const tile = L.tileLayer('tiles/{z}/{x}.{y}.png', {
    tileSize: tileSize,
    bounds: bounds,
    tms: true,
    continuousWorld: true,
    minNativeZoom: 0,
    maxNativeZoom: 4
}).addTo(map);

map.setMaxBounds(bounds); // Définissez les limites maximales de la carte