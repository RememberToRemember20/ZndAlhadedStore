const area = (kind) => (kind === "local" ? window.localStorage : window.sessionStorage);

export function getItem(kind, key) {
    try { return area(kind).getItem(key); } catch { return null; }
}

export function setItem(kind, key, value) {
    try { area(kind).setItem(key, value); return true; } catch { return false; }
}

export function removeItem(kind, key) {
    try { area(kind).removeItem(key); } catch { /* التخزين غير متاح */ }
}

// تسلسل تجديد التوكن بين التبويبات (Web Locks)
export async function withLock(name, target, method) {
    if (navigator.locks && navigator.locks.request) {
        return await navigator.locks.request(name, () => target.invokeMethodAsync(method));
    }
    return await target.invokeMethodAsync(method);
}