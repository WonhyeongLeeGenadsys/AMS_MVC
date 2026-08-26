// electric.js 파일

// DB/API의 CoF와 Risk 원본값은 USD로 유지하고 화면에 표시할 때만 원화로 환산합니다.
window.usdKrwRate = 0;
window.usdKrwRatePromise = null;

window.loadUsdKrwRate = function () {
    if (window.usdKrwRatePromise) return window.usdKrwRatePromise;

    const deferred = $.Deferred();
    window.usdKrwRatePromise = deferred.promise();
    const url = document.body && document.body.getAttribute('data-usdkrw-url');

    if (!url) {
        deferred.reject('USD/KRW 환율 URL이 설정되지 않았습니다.');
        return window.usdKrwRatePromise;
    }

    $.getJSON(url)
        .done(function (result) {
            const rate = Number(result && result.rate);
            if (!Number.isFinite(rate) || rate <= 0) {
                deferred.reject('USD/KRW 환율 응답값이 올바르지 않습니다.');
                return;
            }

            window.usdKrwRate = rate;
            try {
                localStorage.setItem('ams.usdKrwRate', JSON.stringify({ rate: rate, savedAt: Date.now() }));
            } catch (ignore) { }

            if (result.isFallback) {
                console.warn('USD/KRW 대체 환율 적용:', rate + '원', result.message || '');
            } else {
                console.info('USD/KRW 환율:', rate + '원', '(' + result.source + ')');
            }
            deferred.resolve(rate);
        })
        .fail(function (xhr) {
            let cached = null;
            try {
                cached = JSON.parse(localStorage.getItem('ams.usdKrwRate'));
            } catch (ignore) { }

            const cachedRate = Number(cached && cached.rate);
            if (Number.isFinite(cachedRate) && cachedRate > 0) {
                window.usdKrwRate = cachedRate;
                console.warn('환율 조회 실패로 브라우저 저장 환율 적용:', cachedRate + '원');
                deferred.resolve(cachedRate);
                return;
            }

            console.error('USD/KRW 환율 조회 실패', xhr.status, xhr.responseText);
            deferred.reject(xhr);
        });

    return window.usdKrwRatePromise;
};

window.usdToKrw = function (usd) {
    const value = Number(usd);
    return Number.isFinite(value) && window.usdKrwRate > 0
        ? value * window.usdKrwRate
        : 0;
};

window.usdToEokKrw = function (usd) {
    return window.usdToKrw(usd) / 100000000;
};

window.formatKrwFromUsd = function (usd) {
    return Math.round(window.usdToKrw(usd)).toLocaleString('ko-KR');
};

// 장비정보 화면은 동일한 알고리즘 데이터를 여러 차트에서 함께 사용합니다.
// 같은 장비의 동시 GET 요청은 하나로 합치고 각 호출자의 콜백에는 같은 결과를 전달합니다.
(function installEquipmentAlgorithmRequestDeduplication($) {
    if (!$ || $.ajax.__amsEquipmentDeduplicationInstalled) return;

    const originalAjax = $.ajax;
    const pending = Object.create(null);
    const completed = Object.create(null);

    function isSharedAlgorithmRequest(settings) {
        const method = String(settings.method || settings.type || 'GET').toUpperCase();
        const url = String(settings.url || '').toLowerCase();
        return method === 'GET' && url.indexOf('/equipmentalgorithm/getalgorithmdata') >= 0;
    }

    function requestKey(settings) {
        const data = typeof settings.data === 'string'
            ? settings.data
            : $.param(settings.data || {});
        return String(settings.url || '').toLowerCase() + '?' + data;
    }

    function subscribe(shared, settings) {
        const follower = $.Deferred();
        const context = settings.context || settings;

        shared.done(function (data, textStatus, jqXHR) {
            if ($.isFunction(settings.success)) {
                settings.success.call(context, data, textStatus, jqXHR);
            }
            if ($.isFunction(settings.complete)) {
                settings.complete.call(context, jqXHR, textStatus);
            }
            follower.resolveWith(context, [data, textStatus, jqXHR]);
        });
        shared.fail(function (jqXHR, textStatus, errorThrown) {
            if ($.isFunction(settings.error)) {
                settings.error.call(context, jqXHR, textStatus, errorThrown);
            }
            if ($.isFunction(settings.complete)) {
                settings.complete.call(context, jqXHR, textStatus);
            }
            follower.rejectWith(context, [jqXHR, textStatus, errorThrown]);
        });

        const promise = follower.promise();
        promise.abort = function () {
            follower.rejectWith(context, [null, 'abort', 'abort']);
        };
        return promise;
    }

    function cachedAjax(url, options) {
        const settings = typeof url === 'object'
            ? $.extend({}, url)
            : $.extend({}, options || {}, { url: url });

        if (!isSharedAlgorithmRequest(settings)) {
            return originalAjax.apply($, arguments);
        }

        const key = requestKey(settings);
        if (completed[key]) {
            const cached = $.Deferred();
            window.setTimeout(function () {
                cached.resolve(completed[key].data, completed[key].textStatus, completed[key].jqXHR);
            }, 0);
            return subscribe(cached.promise(), settings);
        }

        if (!pending[key]) {
            const sharedSettings = $.extend({}, settings);
            delete sharedSettings.success;
            delete sharedSettings.error;
            delete sharedSettings.complete;

            pending[key] = originalAjax.call($, sharedSettings);
            pending[key].done(function (data, textStatus, jqXHR) {
                completed[key] = { data: data, textStatus: textStatus, jqXHR: jqXHR };
            });
            pending[key].always(function () {
                delete pending[key];
            });
        }

        return subscribe(pending[key], settings);
    }

    cachedAjax.__amsEquipmentDeduplicationInstalled = true;
    $.ajax = cachedAjax;
})(window.jQuery);

window.getRiskMatrixMaxEok = function (points) {
    const values = (points || []).map(function (point) { return Number(point.x || 0); });
    const dataMax = Math.max.apply(Math, [0].concat(values));
    const rawMax = Math.max(0.6, dataMax);
    const interval = rawMax <= 1 ? 0.2 : Math.pow(10, Math.floor(Math.log10(rawMax))) / 2;
    return Math.ceil(rawMax / interval) * interval;
};

// DevExtreme가 실제로 계산한 plot 영역과 Risk Matrix 배경 격자를 일치시킨다.
// 축 라벨 길이, 환율, 화면 크기에 따라 plot 폭이 달라져도 고정 px 값에 의존하지 않는다.
window.alignRiskMatrixBackground = function (chartElement) {
    const chart = typeof chartElement === 'string'
        ? document.querySelector(chartElement)
        : chartElement;

    if (!chart) return false;

    const group = chart.closest('.matrix_group');
    const background = group && group.querySelector('.chartContainer_bg');
    const svg = chart.querySelector('svg');

    if (!group || !background || !svg) return false;

    const svgWidth = (svg.viewBox && svg.viewBox.baseVal && svg.viewBox.baseVal.width)
        || (svg.width && svg.width.baseVal && svg.width.baseVal.value)
        || svg.getBoundingClientRect().width;
    const svgHeight = (svg.viewBox && svg.viewBox.baseVal && svg.viewBox.baseVal.height)
        || (svg.height && svg.height.baseVal && svg.height.baseVal.value)
        || svg.getBoundingClientRect().height;

    const plotRects = Array.prototype.map.call(svg.querySelectorAll('clipPath rect'), function (rect) {
        return {
            x: Number(rect.getAttribute('x')),
            y: Number(rect.getAttribute('y')),
            width: Number(rect.getAttribute('width')),
            height: Number(rect.getAttribute('height'))
        };
    }).filter(function (rect) {
        return Number.isFinite(rect.x)
            && Number.isFinite(rect.y)
            && rect.width > 0
            && rect.height > 0
            && (rect.width < svgWidth || rect.height < svgHeight);
    }).sort(function (a, b) {
        return (b.width * b.height) - (a.width * a.height);
    });

    if (!plotRects.length) return false;

    const plot = plotRects[0];
    const groupRect = group.getBoundingClientRect();
    const svgRect = svg.getBoundingClientRect();
    const scaleX = svgWidth ? svgRect.width / svgWidth : 1;
    const scaleY = svgHeight ? svgRect.height / svgHeight : 1;

    background.style.left = ((svgRect.left - groupRect.left) + (plot.x * scaleX)) + 'px';
    background.style.top = ((svgRect.top - groupRect.top) + (plot.y * scaleY)) + 'px';
    background.style.width = (plot.width * scaleX) + 'px';
    background.style.height = (plot.height * scaleY) + 'px';

    return true;
};

(function setUpRiskMatrixAlignment() {
    let alignmentQueued = false;

    window.alignAllRiskMatrixBackgrounds = function () {
        document.querySelectorAll('.matrix_group .chart_box.matrix').forEach(function (chart) {
            window.alignRiskMatrixBackground(chart);
        });
    };

    window.scheduleRiskMatrixAlignment = function () {
        if (alignmentQueued) return;

        alignmentQueued = true;
        window.requestAnimationFrame(function () {
            alignmentQueued = false;
            window.alignAllRiskMatrixBackgrounds();
        });
    };

    $(document).ready(function () {
        const observeTargets = document.querySelectorAll('.matrix_group');

        if (observeTargets.length && window.MutationObserver) {
            const mutationObserver = new MutationObserver(function (mutations) {
                const chartChanged = mutations.some(function (mutation) {
                    return Array.prototype.some.call(mutation.addedNodes, function (node) {
                        return node.nodeType === 1
                            && (node.matches('.matrix_group, .matrix_group *')
                                || node.querySelector('.matrix_group'));
                    });
                });

                if (chartChanged) window.scheduleRiskMatrixAlignment();
            });

            observeTargets.forEach(function (target) {
                mutationObserver.observe(target, { childList: true, subtree: true });
            });
        }

        if (window.ResizeObserver) {
            const resizeObserver = new ResizeObserver(window.scheduleRiskMatrixAlignment);
            document.querySelectorAll('.matrix_group').forEach(function (group) {
                resizeObserver.observe(group);
            });
        }

        window.addEventListener('resize', window.scheduleRiskMatrixAlignment);
        window.scheduleRiskMatrixAlignment();
        window.setTimeout(window.scheduleRiskMatrixAlignment, 300);
    });
})();

// AJAX 응답과 DevExtreme 위젯이 갑자기 나타나지 않도록 짧은 공통 전환을 적용한다.
// 빠르게 끝나는 요청에는 진행 표시를 띄우지 않아 화면이 깜빡이지 않게 한다.
(function setUpSmoothDataRendering($) {
    if (!$) return;

    let activeRequests = 0;
    let showTimer = null;
    let hideTimer = null;
    const revealed = typeof WeakSet === 'function' ? new WeakSet() : null;
    const revealSelector = [
        '.dx-datagrid',
        '.dx-chart',
        '.dx-piechart',
        '.dx-circulargauge',
        '.dm-v31-kpi',
        '.dm-v31-tab-panel.is-active',
        '.totalInfo_content.on'
    ].join(',');

    function ensureLoadingBar() {
        let bar = document.querySelector('.ams-global-loading-bar');
        if (bar || !document.body) return bar;

        bar = document.createElement('div');
        bar.className = 'ams-global-loading-bar';
        bar.setAttribute('aria-hidden', 'true');
        document.body.appendChild(bar);
        return bar;
    }

    function showLoadingBar() {
        const bar = ensureLoadingBar();
        if (bar) bar.classList.add('is-visible');
    }

    function hideLoadingBar() {
        const bar = ensureLoadingBar();
        if (bar) bar.classList.remove('is-visible');
    }

    function beginRequest(settings) {
        if (settings && settings.amsSilentLoading === true) return;

        activeRequests += 1;
        window.clearTimeout(hideTimer);
        if (activeRequests !== 1) return;

        window.clearTimeout(showTimer);
        showTimer = window.setTimeout(showLoadingBar, 160);
    }

    function endRequest(settings) {
        if (settings && settings.amsSilentLoading === true) return;

        activeRequests = Math.max(0, activeRequests - 1);
        if (activeRequests > 0) return;

        window.clearTimeout(showTimer);
        hideTimer = window.setTimeout(hideLoadingBar, 100);
    }

    function reveal(element) {
        if (!element || element.nodeType !== 1) return;
        if (revealed && revealed.has(element)) return;
        if (element.dataset && element.dataset.amsSmoothRevealed === 'true') return;

        if (revealed) revealed.add(element);
        if (element.dataset) element.dataset.amsSmoothRevealed = 'true';
        element.classList.remove('ams-data-reveal');
        // 같은 프레임에서 만들어진 여러 위젯은 한 번의 페인트에서 함께 표시한다.
        window.requestAnimationFrame(function () {
            element.classList.add('ams-data-reveal');
        });
    }

    function replayReveal(element) {
        if (!element || element.nodeType !== 1) return;

        element.classList.remove('ams-data-reveal');
        // 기존 애니메이션을 종료한 뒤 같은 탭을 다시 열어도 전환이 재실행되게 한다.
        void element.offsetWidth;
        element.classList.add('ams-data-reveal');
    }

    function getTabTarget(trigger) {
        if (!trigger) return null;

        let target = trigger.getAttribute('data-tab')
            || trigger.getAttribute('data-target')
            || trigger.getAttribute('data-bs-target');

        if (!target && trigger.matches('[data-toggle="tab"], [data-bs-toggle="tab"]')) {
            target = trigger.getAttribute('href');
        }

        if (!target) return null;
        target = String(target).trim();
        if (!target) return null;

        if (target.charAt(0) === '#') {
            try {
                return document.querySelector(target);
            } catch (_ignore) {
                return null;
            }
        }

        return document.getElementById(target);
    }

    function enterPage() {
        const content = document.querySelector('.content_group');
        if (!content) return;

        content.classList.remove('ams-page-leave', 'ams-page-enter');
        void content.offsetWidth;
        content.classList.add('ams-page-enter');
    }

    function isInternalMenuLink(event, link) {
        if (!link || event.defaultPrevented || event.button !== 0) return false;
        if (event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return false;
        if (link.hasAttribute('download')) return false;
        if (link.target && link.target.toLowerCase() !== '_self') return false;
        if (!link.closest('header, .menu_group')) return false;

        const rawHref = String(link.getAttribute('href') || '').trim();
        if (!rawHref || rawHref.charAt(0) === '#') return false;
        if (/^(javascript:|mailto:|tel:)/i.test(rawHref)) return false;

        let destination;
        try {
            destination = new URL(link.href, window.location.href);
        } catch (_ignore) {
            return false;
        }

        return destination.origin === window.location.origin;
    }

    function scanForReveal(root) {
        if (!root || root.nodeType !== 1) return;

        if (root.matches(revealSelector)) reveal(root);
        const closest = root.closest(revealSelector);
        if (closest) reveal(closest);
        root.querySelectorAll(revealSelector).forEach(reveal);
    }

    $(document)
        .on('ajaxSend.amsSmoothData', function (_event, _xhr, settings) {
            beginRequest(settings);
        })
        .on('ajaxComplete.amsSmoothData', function (_event, _xhr, settings) {
            endRequest(settings);
        });

    $(function () {
        ensureLoadingBar();
        enterPage();
        scanForReveal(document.body);

        document.addEventListener('click', function (event) {
            const eventTarget = event.target && event.target.nodeType === 1
                ? event.target
                : event.target && event.target.parentElement;
            if (!eventTarget) return;

            const tabTrigger = eventTarget.closest(
                '[data-tab], [role="tab"][data-target], [data-toggle="tab"], [data-bs-toggle="tab"]'
            );
            if (tabTrigger) {
                const target = getTabTarget(tabTrigger);
                window.requestAnimationFrame(function () {
                    replayReveal(target);
                });
                return;
            }

            const link = eventTarget.closest('a[href]');
            if (!isInternalMenuLink(event, link)) return;

            event.preventDefault();
            const content = document.querySelector('.content_group');
            if (content) {
                content.classList.remove('ams-page-enter');
                content.classList.add('ams-page-leave');
            }
            showLoadingBar();

            window.setTimeout(function () {
                window.location.assign(link.href);
            }, 100);
        });

        window.addEventListener('pageshow', function (event) {
            if (event.persisted) enterPage();
        });

        const content = document.querySelector('.content_group') || document.body;
        if (!content || !window.MutationObserver) return;

        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === 'attributes') {
                    scanForReveal(mutation.target);
                    return;
                }

                Array.prototype.forEach.call(mutation.addedNodes, scanForReveal);
            });
        });

        observer.observe(content, {
            childList: true,
            subtree: true,
            attributes: true,
            attributeFilter: ['class']
        });
    });
})(window.jQuery);

// 페이지 로드 및 메뉴 활성화
$(document).ready(function () {
    const menuType = $('body').data('menutype');

    if (!menuType) {
        return;
    }

    switch (menuType) {
        case 'TotalInfo': // 종합정보
            $('#gnb_01').addClass('on');
            break;
        case 'DeviceInfo': // 장치 정보
            $('#gnb_02').addClass('on');
            break;
        case 'Regist': // 등록
            $('#gnb_03').addClass('on');
            break;
        case 'Check': // 점검
            $('#gnb_04').addClass('on');
            break;
        case 'Gojang': // 고장
            $('#gnb_05').addClass('on');
            break;
        case 'Maintenance': // 유지보수
            $('#gnb_06').addClass('on');
            break;

        case 'SPARE': // 예비품
            $('#gnb_07').addClass('on');
            break;

        default:
            break;
    }

    // 레프트 메뉴 활성화 함수 호출
    activateMenuItem();

    // 레프트 메뉴 활성화 함수 (VCB, ITR 등을 포함하여 비교)
    function activateMenuItem() {
        const currentPage = location.pathname.split('/').pop().split('.')[0];

        $('.menu_group a').each(function () {
            const href = $(this).attr("href");
            const linkPage = href.split('/').pop().split('.')[0];

            if (currentPage === linkPage) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("VCB") && linkPage.includes("VCB")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("ITR") && linkPage.includes("ITR")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("DCCB") && linkPage.includes("DCCB")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("DCCABLE") && linkPage.includes("DCCABLE")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("SUBMODULE") && linkPage.includes("SUBMODULE")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("CofInfo") && linkPage.includes("CofInfo")) {
                $(this).addClass('on');
            }
            else if (currentPage.includes("SUBSTATION") && linkPage.includes("SUBSTATION")) {
                $(this).addClass('on');
            }

        });
    }
});

// 메뉴 숨기기/보이기 토글
$('.btn_menu').click(function () {
    $('.menu_group').toggle(); // 좌측 메뉴 영역을 토글
    // 필요하다면 버튼 텍스트도 변경
    if ($('.menu_group').is(':visible')) {
        $(this).text('메뉴 숨기기');
    } else {
        $(this).text('메뉴 보이기');
    }
});

// 팝업
function openPop(url, w, h) {
    const win = window.open(url, "openpopup", "width=" + w + ",height=" + h + ",top=10,left=10");
    if (!win) window.open(url, "_blank");
}

// 파일 업로드
function fileChange() {
    const curEl = document.querySelector('#fileUpload').value.split("\\").pop();
    const fileNameEl = document.querySelector('#fileNm');
    fileNameEl.value = curEl;
}

// 탑 메뉴 마우스 오버시 활성화
const menuItems = document.querySelectorAll('.gnb_group > li');
const showSubmenu = function () {
    const submenuEl = this.querySelector('.lnb_group');
    if (submenuEl) {
        submenuEl.classList.add('on');
    }
}
const hideSubmenu = function () {
    const submenuEl = this.querySelector('.lnb_group');
    if (submenuEl) {
        submenuEl.classList.remove('on');
    }
}
menuItems.forEach((menuItem) => {
    menuItem.addEventListener('mouseenter', showSubmenu);
    menuItem.addEventListener('mouseleave', hideSubmenu);
    menuItem.addEventListener('wheel', event => {
        event.preventDefault();
    });
});

const settingMenu = document.querySelector('.setting');
const submenuEl = document.querySelector('.setting_sub');
settingMenu.addEventListener('mouseenter', () => {
    submenuEl.classList.add('on');
});
settingMenu.addEventListener('mouseleave', () => {
    submenuEl.classList.remove('on');
});

document.addEventListener('input', function (event) {
    const target = event.target;

    // 숫자만 입력
    if (target.dataset.type === 'number') {
        target.value = target.value.replace(/[^0-9.]/g, ''); // 숫자와 '.'만 허용
    }

    // 문자만 입력
    if (target.dataset.type === 'string') {
        target.value = target.value.replace(/[^a-zA-Z0-9_-]/g, ''); // 영문, 숫자, 하이픈, 밑줄만 허용
    }
});

//텝 클릭시 활성화
document.querySelectorAll('.tab_twodepth').forEach(tabGroup => {
    const tabBtns = tabGroup.querySelectorAll('.tab_btn');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            // 현재 클릭한 버튼의 data-tab 값으로 대상 콘텐츠 id 찾기
            const targetId = btn.getAttribute('data-tab');
            const targetContent = document.querySelector('#' + targetId);

            // 같은 그룹 내에서만 탭 버튼 활성화 조작
            tabBtns.forEach(b => b.classList.remove('on'));
            btn.classList.add('on');

            // 이 탭 그룹과 관련된 콘텐츠들만 찾아서 조작
            // (탭 그룹과 콘텐츠가 같은 부모 요소를 공유한다고 가정)
            const parent = tabGroup.parentElement;

            const contents = parent.querySelectorAll('.totalInfo_content');
            contents.forEach(c => c.classList.remove('on'));

            if (targetContent) {
                targetContent.classList.add('on');
            }

            if (window.scheduleRiskMatrixAlignment) {
                window.scheduleRiskMatrixAlignment();
            }
        });
    });
});

$(document).ready(function () {

    // 1-depth 탭 클릭 처리
    $('.tab_onedepth .tab_btn').on('click', function () {
        // 1) 탭 하이라이트
        $('.tab_onedepth .tab_btn').removeClass('on');
        $(this).addClass('on');

        // 2) 해당 컨텐츠 보이기
        var targetId = $(this).data('tab');  
        $('.box_wrap.totalInfo_content').removeClass('on');
        $('#' + targetId).addClass('on');

        if (window.scheduleRiskMatrixAlignment) {
            window.scheduleRiskMatrixAlignment();
        }

    });
});
