// Small browser helpers for Imeji. Imported on demand by Services/ImejiJs.cs.

export function setCookie(name, value) {
    document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`;
}

// Local storage can throw (private windows, blocked site data), so treat it as optional.
export function storageGet(key) {
    try {
        return localStorage.getItem(key);
    } catch {
        return null;
    }
}

export function storageSet(key, value) {
    try {
        localStorage.setItem(key, value);
    } catch {
        // Nothing to do: saved and recent searches just won't survive a reload.
    }
}

function isTyping(target) {
    return target instanceof HTMLElement
        && (target.isContentEditable || ['INPUT', 'TEXTAREA', 'SELECT'].includes(target.tagName));
}

export function registerShortcuts(dotNetHelper) {
    const onKeyDown = (event) => {
        const mod = event.ctrlKey || event.metaKey;
        let key;

        if (mod && event.key.toLowerCase() === 'k') {
            key = 'mod+k';
        } else if (mod || event.altKey) {
            return;
        } else if (isTyping(event.target) && event.key !== 'Escape') {
            return;
        } else {
            key = event.key.length === 1 ? event.key.toLowerCase() : event.key;
        }

        if (key === 'mod+k' || key === '/') {
            event.preventDefault();
        }

        dotNetHelper.invokeMethodAsync('OnKey', key);
    };

    document.addEventListener('keydown', onKeyDown);

    // Marks the page as interactive; the end-to-end tests wait for it before clicking.
    document.documentElement.dataset.interactive = '';

    return { dispose: () => document.removeEventListener('keydown', onKeyDown) };
}

// Reports the [data-page] marker closest above the upper third of the screen, once per change.
export function trackPages(dotNetHelper) {
    let current = 0;
    let frame = 0;

    const update = () => {
        frame = 0;
        const line = window.innerHeight / 3;
        let page = 0;
        for (const marker of document.querySelectorAll('[data-page]')) {
            if (marker.getBoundingClientRect().top <= line) {
                page = Number(marker.dataset.page);
            }
        }

        if (page && page !== current) {
            current = page;
            dotNetHelper.invokeMethodAsync('SetCurrentPage', page);
        }
    };

    const onScroll = () => {
        if (!frame) {
            frame = requestAnimationFrame(update);
        }
    };

    window.addEventListener('scroll', onScroll, { passive: true });
    update();

    return {
        dispose: () => {
            window.removeEventListener('scroll', onScroll);
            cancelAnimationFrame(frame);
        },
    };
}

export function attachPaletteKeys(input, dotNetHelper) {
    const handled = ['ArrowUp', 'ArrowDown', 'Tab', 'Enter', 'Escape'];

    const onKeyDown = (event) => {
        // Backspace in an empty input removes the last tag chip, like most tag inputs.
        if (event.key === 'Backspace' && input.value === '') {
            dotNetHelper.invokeMethodAsync('OnPaletteKey', 'Backspace', false);
            return;
        }

        if (!handled.includes(event.key)) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
        dotNetHelper.invokeMethodAsync('OnPaletteKey', event.key, event.shiftKey);
    };

    input.addEventListener('keydown', onKeyDown);
    return { dispose: () => input.removeEventListener('keydown', onKeyDown) };
}

// Scroll positions per URL, saved when a link is followed, so coming back can return to the same spot.
// Saved on click rather than on scroll: by the time a scroll event fires after navigating, location is already the new page.
const scrollPositions = new Map();

export function rememberScrollOnLeave() {
    const onClick = (event) => {
        if (event.target instanceof Element && event.target.closest('a[href]')) {
            scrollPositions.set(location.href, window.scrollY);
        }
    };

    document.addEventListener('click', onClick, true);
    return { dispose: () => document.removeEventListener('click', onClick, true) };
}

export function restoreScroll() {
    const y = scrollPositions.get(location.href);
    if (y === undefined) {
        return false;
    }

    window.scrollTo({ top: y, behavior: 'instant' });
    return true;
}

// Reports one-finger swipes on an element as "left", "right" or "up" through a JSInvokable OnSwipe(string).
// Pinches and mostly-diagonal drags are ignored, so zooming and scrolling still work.
export function attachSwipe(element, dotNetHelper) {
    let start = null;

    const onStart = (event) => {
        start = event.touches.length === 1 ? { x: event.touches[0].clientX, y: event.touches[0].clientY } : null;
    };

    const onEnd = (event) => {
        if (!start || event.changedTouches.length !== 1) {
            return;
        }

        const dx = event.changedTouches[0].clientX - start.x;
        const dy = event.changedTouches[0].clientY - start.y;
        start = null;

        if (Math.abs(dx) > 60 && Math.abs(dx) > Math.abs(dy) * 1.5) {
            dotNetHelper.invokeMethodAsync('OnSwipe', dx < 0 ? 'left' : 'right');
        } else if (dy < -80 && Math.abs(dy) > Math.abs(dx) * 1.5) {
            dotNetHelper.invokeMethodAsync('OnSwipe', 'up');
        }
    };

    element.addEventListener('touchstart', onStart, { passive: true });
    element.addEventListener('touchend', onEnd, { passive: true });

    return {
        dispose: () => {
            element.removeEventListener('touchstart', onStart);
            element.removeEventListener('touchend', onEnd);
        },
    };
}

// Warms the browser cache for images the user is likely to open next.
export function preloadImages(urls) {
    for (const url of urls) {
        if (url) {
            new Image().src = url;
        }
    }
}

// Whether feed videos play muted. Remembered for the browser session, so unmuting one video unmutes the next ones.
const feedMutedKey = 'imeji-feed-muted';

function isFeedMuted() {
    try {
        return sessionStorage.getItem(feedMutedKey) !== 'false';
    } catch {
        return true;
    }
}

function setFeedMuted(muted) {
    try {
        sessionStorage.setItem(feedMutedKey, String(muted));
    } catch {
        // Not remembered; videos just start muted again.
    }
}

// Sets muted ourselves and notes it, so the volumechange it causes is not mistaken for the user's choice.
function applyMuted(video, muted) {
    video.dataset.expectedMuted = String(muted);
    video.muted = muted;
}

// Plays feed videos while they fill most of the screen and pauses them when scrolled away. They play muted
// until the user unmutes one (see isFeedMuted). They start without controls so nothing covers the video;
// a tap shows the player controls, and scrolling away hides them again. Watches for slides added later.
export function autoplayVideos() {
    const visibility = new IntersectionObserver((entries) => {
        for (const entry of entries) {
            const video = entry.target;
            if (entry.isIntersecting) {
                applyMuted(video, isFeedMuted());
                video.play().catch(() => {
                    // Browsers may refuse to start sound without a tap; play muted instead, keeping the preference.
                    applyMuted(video, true);
                    video.play().catch(() => { /* Autoplay blocked entirely; a tap still plays it. */ });
                });
            } else {
                video.pause();
                video.controls = false;
            }
        }
    }, { threshold: 0.6 });

    const onClick = (event) => {
        const video = event.target instanceof Element ? event.target.closest('.feed-slide video') : null;
        if (video && !video.controls) {
            video.controls = true;
        }
    };

    // Only a change the user made (through the player controls) updates the remembered setting.
    const onVolumeChange = (event) => {
        const video = event.target;
        if (!(video instanceof HTMLVideoElement) || !video.closest('.feed-slide')) {
            return;
        }

        if (String(video.muted) !== video.dataset.expectedMuted) {
            setFeedMuted(video.muted);
        }

        video.dataset.expectedMuted = String(video.muted);
    };

    document.addEventListener('click', onClick);
    document.addEventListener('volumechange', onVolumeChange, true);

    const observeAll = () => document.querySelectorAll('.feed-slide video:not([data-autoplay])').forEach(video => {
        video.dataset.autoplay = '';
        visibility.observe(video);
    });

    const additions = new MutationObserver(observeAll);
    additions.observe(document.body, { childList: true, subtree: true });
    observeAll();

    return {
        dispose: () => {
            additions.disconnect();
            visibility.disconnect();
            document.removeEventListener('click', onClick);
            document.removeEventListener('volumechange', onVolumeChange, true);
        },
    };
}

export function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

export function scrollIntoView(selector, block, smooth) {
    document.querySelector(selector)?.scrollIntoView({ behavior: smooth ? 'smooth' : 'instant', block });
}

export function setScrollLock(locked) {
    document.documentElement.classList.toggle('scroll-locked', locked);
}

export function toggleFullscreen(element) {
    if (document.fullscreenElement) {
        document.exitFullscreen();
    } else {
        element?.requestFullscreen?.();
    }
}

export async function copyText(text) {
    await navigator.clipboard.writeText(text);
}

export async function share(url, title) {
    if (navigator.share) {
        try {
            await navigator.share({ url, title });
            return 'shared';
        } catch {
            // Cancelled by the user; fall back to copying so the button still does something.
        }
    }

    await navigator.clipboard.writeText(url);
    return 'copied';
}

// Moves the feed one slide up or down from the slide currently filling the screen.
export function stepFeed(direction) {
    const slides = [...document.querySelectorAll('.feed-slide')];
    const middle = window.innerHeight / 2;
    const index = slides.findIndex(slide => {
        const rect = slide.getBoundingClientRect();
        return rect.top <= middle && rect.bottom > middle;
    });

    slides[Math.max(0, index + direction)]?.scrollIntoView({ behavior: 'smooth', block: 'start' });
}
