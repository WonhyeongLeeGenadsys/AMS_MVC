// electric.js 파일

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
        default:
            break;
    }

    // 레프트 메뉴 활성화 함수 호출
    activateMenuItem();

    // 레프트 메뉴 활성화 함수 (VCB, ITR 등을 포함하여 비교)
    function activateMenuItem() {
        // 현재 페이지의 파일명 추출 (ex: "VCBList" || "ITRBasicList")
        const currentPage = location.pathname.split('/').pop().split('.')[0];

        $('.menu_group a').each(function () {
            // 각 링크의 파일명 추출
            const href = $(this).attr("href");
            const linkPage = href.split('/').pop().split('.')[0];

            // 정확히 일치하면 'on' 클래스 추가
            if (currentPage === linkPage) {
                $(this).addClass('on');
            }
            // "VCB"가 포함시
            else if (currentPage.includes("VCB") && linkPage.includes("VCB")) {
                $(this).addClass('on');
            }
            // "ITR"가 포함시
            else if (currentPage.includes("ITR") && linkPage.includes("ITR")) {
                $(this).addClass('on');
            }
            // 향후 장치 추가되면 "DCCB, DC Cable 등등 추가하기!
        });
    }
});

// 팝업
function openPop(url, w, h) {
    window.open(url, "openpopup", "width=" + w + ", height=" + h + ", top=10, left=10");
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
