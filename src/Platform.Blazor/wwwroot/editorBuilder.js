window.platformBuilder = {
    getRect(element) {
        if (!element) {
            return { left: 0, top: 0, width: 0, height: 0 };
        }

        const rect = element.getBoundingClientRect();
        return {
            left: rect.left,
            top: rect.top,
            width: rect.width,
            height: rect.height
        };
    },
    capturePointer(element, pointerId) {
        try {
            element?.setPointerCapture?.(pointerId);
        } catch {
        }
    },
    releasePointer(element, pointerId) {
        try {
            element?.releasePointerCapture?.(pointerId);
        } catch {
        }
    },
    focusElement(element) {
        element?.focus?.({ preventScroll: true });
    }
};
