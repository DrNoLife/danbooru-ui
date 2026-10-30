export function observe(sentinel, dotNetHelper) {
    let isLoading = false;

    const observer = new IntersectionObserver(async (entries) => {
        if (isLoading || !entries.some(entry => entry.isIntersecting)) {
            return;
        }

        isLoading = true;
        try {
            const loadedMore = await dotNetHelper.invokeMethodAsync('OnSentinelVisible');

            // Observing again reports the current state, so a sentinel that is still visible
            // (e.g. a short page) triggers the next load. Stop when nothing more was loaded.
            observer.unobserve(sentinel);
            if (loadedMore) {
                observer.observe(sentinel);
            }
        } finally {
            isLoading = false;
        }
    }, { rootMargin: '0px 0px 800px 0px' });

    observer.observe(sentinel);

    return {
        dispose: () => observer.disconnect(),
    };
}
