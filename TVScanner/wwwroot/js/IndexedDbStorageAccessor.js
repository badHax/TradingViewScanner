export function initialize() {
    let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);
    scannerIndexedDb.onupgradeneeded = function () {
        let db = scannerIndexedDb.result;
        db.createObjectStore("scanRecords", { keyPath: "name" });
    }
}

export function set(collectionName, value) {
    let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    scannerIndexedDb.onsuccess = function () {
        let transaction = scannerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName)
        collection.put(value);
    }
}

export function setMany(collectionName, values) {
    let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    scannerIndexedDb.onsuccess = function () {
        let transaction = scannerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName);

        values.forEach(value => {
            collection.put(value);
        });
    }
}

export function purge(collectionName, ids) {
    let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    scannerIndexedDb.onsuccess = function () {
        let transaction = scannerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName);

        ids.forEach(id => {
            collection.delete(id);
        });
    }
}

export async function get(collectionName, id) {
    let request = new Promise((resolve) => {
        let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);
        scannerIndexedDb.onsuccess = function () {
            let transaction = scannerIndexedDb.result.transaction(collectionName, "readonly");
            let collection = transaction.objectStore(collectionName);
            let result = collection.get(id);

            result.onsuccess = function (e) {
                resolve(result.result);
            }
        }
    });

    let result = await request;

    return result;
}

export async function getAll(collectionName) {

    let request = new Promise((resolve) => {
        let scannerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);
        scannerIndexedDb.onsuccess = function () {
            let transaction = scannerIndexedDb.result.transaction(collectionName, "readonly");
            let collection = transaction.objectStore(collectionName);
            let result = collection.getAll();

            result.onsuccess = function (e) {
                resolve(result.result);
            }
        }
    });

    let result = await request;

    return result;
}

let CURRENT_VERSION = 1;
let DATABASE_NAME = "Scanner Records";