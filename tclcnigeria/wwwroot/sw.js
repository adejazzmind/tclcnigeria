const CACHE_NAME = 'tclcnigeria-v2';
const OFFLINE_URL = '/offline.html';

const STATIC_ASSETS = [
  '/',
  '/Home/About',
  '/Home/Ministries',
  '/Home/Sermons',
  '/Home/Contact',
  '/Prayer',
  '/Give',
  '/css/site.css',
  '/lib/bootstrap/dist/css/bootstrap.min.css',
  '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
  '/lib/jquery/dist/jquery.min.js'
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => {
      return cache.addAll(STATIC_ASSETS);
    }).then(() => self.skipWaiting())
  );
});

self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames
          .filter(name => name !== CACHE_NAME)
          .map(name => caches.delete(name))
      );
    }).then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', event => {
  if (event.request.method !== 'GET') return;

  // NEVER cache Identity/auth pages or admin pages
  const url = new URL(event.request.url);
  if (url.pathname.includes('/Identity/') ||
      url.pathname.includes('/Admin') ||
      url.pathname.includes('/Login') ||
      url.pathname.includes('/Register') ||
      url.pathname.includes('/Logout') ||
      url.pathname.includes('/Prayer') ||
      url.pathname.includes('/Give') ||
      url.pathname.includes('/Contact')) {
    return;
  }

  event.respondWith(
    caches.match(event.request).then(cachedResponse => {
      if (cachedResponse) return cachedResponse;

      return fetch(event.request).then(networkResponse => {
        if (!networkResponse || networkResponse.status !== 200 || networkResponse.type === 'opaque') {
          return networkResponse;
        }
        const responseToCache = networkResponse.clone();
        caches.open(CACHE_NAME).then(cache => {
          cache.put(event.request, responseToCache);
        });
        return networkResponse;
      }).catch(() => {
        if (event.request.destination === 'document') {
          return caches.match(OFFLINE_URL);
        }
      });
    })
  );
});
