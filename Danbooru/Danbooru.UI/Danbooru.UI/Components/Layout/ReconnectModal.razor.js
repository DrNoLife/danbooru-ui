// Drives the reconnect dialog from Blazor's reconnect events. Blazor adds a state class to the dialog
// (components-reconnect-show, -retrying, -failed, -paused, -resume-failed); app.css shows the matching text.

const modal = document.getElementById('components-reconnect-modal');
modal.addEventListener('components-reconnect-state-changed', onStateChanged);
document.getElementById('components-reconnect-button').addEventListener('click', retry);
document.getElementById('components-resume-button').addEventListener('click', resume);

function onStateChanged(event) {
    switch (event.detail.state) {
        case 'show':
            modal.showModal();
            break;
        case 'hide':
            modal.close();
            break;
        case 'failed':
            // Phones drop the connection while asleep; try again as soon as the page is visible.
            document.addEventListener('visibilitychange', retryWhenVisible);
            break;
        case 'rejected':
            // The server no longer knows this session, so only a reload helps.
            location.reload();
            break;
    }
}

async function retry() {
    document.removeEventListener('visibilitychange', retryWhenVisible);

    try {
        if (await Blazor.reconnect()) {
            return;
        }

        if (Blazor.resumeCircuit && await Blazor.resumeCircuit()) {
            modal.close();
        } else {
            location.reload();
        }
    } catch {
        document.addEventListener('visibilitychange', retryWhenVisible);
    }
}

async function resume() {
    try {
        if (!await Blazor.resumeCircuit()) {
            location.reload();
        }
    } catch {
        location.reload();
    }
}

async function retryWhenVisible() {
    if (document.visibilityState === 'visible') {
        await retry();
    }
}
