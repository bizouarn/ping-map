const tileSize = 265;
const bounds = [[0 * tileSize, 0 * tileSize],[-1 * tileSize, 1 * tileSize]];

const map = L.map('map', {
    crs: L.CRS.Simple
}).setView([0, 0], 0);

const tile = L.tileLayer('tiles/{z}/{y}.{x}.png', {
    tileSize: tileSize,
    bounds: bounds,
    tms: true,
    continuousWorld: true,
    minNativeZoom: 0,
    maxNativeZoom: 8
}).addTo(map);

map.setMaxBounds(bounds); // Définissez les limites maximales de la carte

setTimeout(() => { 
  map.invalidateSize() // see https://github.com/Leaflet/Leaflet/issues/690
}, 100)
