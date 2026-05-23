(function initPlatformBuilder(global) {
    const EMPTY_RECT = Object.freeze({ left: 0, top: 0, width: 0, height: 0 });

    function isElement(value) {
        return value instanceof Element;
    }

    function toFiniteNumber(value) {
        const numeric = typeof value === 'number' ? value : Number(value);
        return Number.isFinite(numeric) ? numeric : null;
    }

    function canUsePointerCapture(element, pointerId) {
        return isElement(element)
            && pointerId !== null
            && typeof element.setPointerCapture === 'function'
            && typeof element.releasePointerCapture === 'function';
    }

    function readRect(element) {
        if (!isElement(element) || typeof element.getBoundingClientRect !== 'function') {
            return EMPTY_RECT;
        }

        const rect = element.getBoundingClientRect();
        return {
            left: Number.isFinite(rect.left) ? rect.left : 0,
            top: Number.isFinite(rect.top) ? rect.top : 0,
            width: Number.isFinite(rect.width) ? rect.width : 0,
            height: Number.isFinite(rect.height) ? rect.height : 0
        };
    }

    function tryCapturePointer(element, pointerId) {
        const numericPointerId = toFiniteNumber(pointerId);
        if (!canUsePointerCapture(element, numericPointerId)) {
            return false;
        }

        try {
            element.setPointerCapture(numericPointerId);
            return true;
        } catch {
            return false;
        }
    }

    function tryReleasePointer(element, pointerId) {
        const numericPointerId = toFiniteNumber(pointerId);
        if (!canUsePointerCapture(element, numericPointerId)) {
            return false;
        }

        if (typeof element.hasPointerCapture === 'function' && !element.hasPointerCapture(numericPointerId)) {
            return false;
        }

        try {
            element.releasePointerCapture(numericPointerId);
            return true;
        } catch {
            return false;
        }
    }

    function focusElement(element) {
        if (!isElement(element) || typeof element.focus !== 'function') {
            return false;
        }

        try {
            element.focus({ preventScroll: true });
            return true;
        } catch {
            element.focus();
            return true;
        }
    }

    global.platformBuilder = {
        getRect: readRect,
        capturePointer: tryCapturePointer,
        releasePointer: tryReleasePointer,
        focusElement
    };
})(window);
