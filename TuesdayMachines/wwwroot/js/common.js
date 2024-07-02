const friendlyHttpStatus = {
    '200': 'OK',
    '201': 'Created',
    '202': 'Accepted',
    '203': 'Non-Authoritative Information',
    '204': 'No Content',
    '205': 'Reset Content',
    '206': 'Partial Content',
    '300': 'Multiple Choices',
    '301': 'Moved Permanently',
    '302': 'Found',
    '303': 'See Other',
    '304': 'Not Modified',
    '305': 'Use Proxy',
    '306': 'Unused',
    '307': 'Temporary Redirect',
    '400': 'Bad Request',
    '401': 'Unauthorized',
    '402': 'Payment Required',
    '403': 'Forbidden',
    '404': 'Not Found',
    '405': 'Method Not Allowed',
    '406': 'Not Acceptable',
    '407': 'Proxy Authentication Required',
    '408': 'Request Timeout',
    '409': 'Conflict',
    '410': 'Gone',
    '411': 'Length Required',
    '412': 'Precondition Required',
    '413': 'Request Entry Too Large',
    '414': 'Request-URI Too Long',
    '415': 'Unsupported Media Type',
    '416': 'Requested Range Not Satisfiable',
    '417': 'Expectation Failed',
    '418': 'I\'m a teapot',
    '429': 'Too Many Requests',
    '500': 'Internal Server Error',
    '501': 'Not Implemented',
    '502': 'Bad Gateway',
    '503': 'Service Unavailable',
    '504': 'Gateway Timeout',
    '505': 'HTTP Version Not Supported',
};

const englishTranslation = {
    'twitch_connect_error': 'Twitch connect error'
};

function translateCode(code) {
    return englishTranslation[code] ?? code;
}

function showMessagesFromUrl() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    if (urlParams.has('success')) {
        toastr.success(translateCode(urlParams.get('success')));
    }

    if (urlParams.has('error')) {
        toastr.error(translateCode(urlParams.get('error')));
    }
}

function getPostRequestOptions(jsonData) {
    return {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'X-Csrf-Token-Value': csrfToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer',
        body: JSON.stringify(jsonData)
    };
}

function getGetRequestOptions() {
    return {
        method: 'GET',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'X-Csrf-Token-Value': csrfToken
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer'
    };
}

function confirmObjectDelete(e) {
    e.setAttribute('disabled', '');

    fetch(e.dataset.url, getPostRequestOptions({
        'id': e.dataset.id
    })).then((response) => response.json())
        .then((json) => {
            if (json.error) {
                toastr.error(translateCode(json.error));
                return;
            }
            toastr.success(translateCode(json.success));
            if (e.dataset.datatable) {
                window[e.dataset.datatable].ajax.reload();
            }
        }).catch((error) => {
            console.error(error);
        }).finally(() => {
            $('#confirmModal').modal('hide');
            e.removeAttribute('disabled');
        });
}

async function getApiGetResult(url) {
    try {
        const response = await fetch(url, getGetRequestOptions());
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();
        if (json && json.error) {
            toastr.error(translateCode(json.error));
            return null;
        }

        return json;
    } catch (error) {
        toastr.error(error);
        console.error(error);
    }

    return null;
}

async function getApiPostResult(url, data) {
    try {
        const response = await fetch(url, getPostRequestOptions(data));
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();
        if (json && json.error) {
            toastr.error(translateCode(json.error));
            return null;
        }

        return json;
    } catch (error) {
        toastr.error(error);
        console.error(error);
    }

    return null;
}

function escapeValue(s) {
    if (!s) {
        return '';
    }

    const lookup = {
        '&': "&amp;",
        '"': "&quot;",
        '\'': "&apos;",
        '<': "&lt;",
        '>': "&gt;"
    };
    return s.replace(/[&"'<>]/g, c => lookup[c]);
}

(function () {
    const forms = document.querySelectorAll('form');
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            if (!form.dataset.url)
                return;

            form.addEventListener('submit', function (event) {
                event.submitter.setAttribute('disabled', '');

                event.preventDefault();
                event.stopPropagation();

                let jsonData = {};
                const inputs = event.target.querySelectorAll('input, select, textarea');
                for (let i = 0, len = inputs.length; i < len; i++) {
                    const input = inputs[i];
                    const name = input.dataset.formName;
                    const type = input.dataset.formType;
                    const convertType = input.dataset.formConvertType;
                    if (!name)
                        continue;

                    if (type) {
                        if (type == 'bool')
                            jsonData[name] = input.checked;
                        else if (type == 'date') {
                            const dt = new Date(input.value || Date.now());
                            jsonData[name] = dt.getTime() / 1000;
                        }
                    } else {
                        jsonData[name] = input.value;
                    }

                    if (convertType) {
                        if (convertType == 'float')
                            jsonData[name] = parseFloat(jsonData[name]);
                        else if (convertType == 'bool')
                            jsonData[name] = jsonData[name] == true;
                    }
                }

                if (event.target.dataset.callbackBefore)
                    jsonData = window[event.target.dataset.callbackBefore](jsonData);

                const isRaw = event.target.dataset.raw == 'true';

                fetch(event.target.dataset.url, getPostRequestOptions(jsonData))
                    .then((response) => {
                        if (isRaw) {
                            if (response.status < 200 || response.status >= 300) {
                                toastr.error(friendlyHttpStatus[response.status] ?? 'Unknown');
                            }
                            return response.text();
                        }
                        return response.json();
                    }).then((json) => {
                        if (!isRaw && json.error) {
                            toastr.error(translateCode(json.error));
                            return;
                        }

                        if (json) {
                            if (event.target.dataset.callback)
                                window[event.target.dataset.callback](json);
                            else if (json.success)
                                toastr.success(translateCode(json.success));
                        }

                    }).catch((error) => {
                        toastr.error(translateCode('server_error'));
                        console.error(error);
                    })
                    .finally(() => {
                        event.submitter.removeAttribute('disabled');
                    });
            }, false)
        });
})();
