// electric.js 파일

// 페이지 로드 및 메뉴 활성화
$(document).ready(function () {
    // 현재 페이지의 경로에서 파일 이름 추출
    const currentPath = location.pathname; // ex) "/Regist/VCB/VCBBasicInfoDetail"
    const lastPageName = currentPath.split('/').pop().split('.')[0]; // ex) "VCBList"
    const topmenuActivePath = currentPath.split('/')[1]; // "Regist"
    const leftMenuActivePath = currentPath.split('/')[2]; // "VCB, VCBChk"


    // 상단 메뉴 활성화
    switch (topmenuActivePath) {
        case 'TotalInfo': //종합정보
            $('#gnb_01').addClass('on');
            break;

        case 'DeviceInfo': //정보
            $('#gnb_02').addClass('on');
            break;

        case 'Regist': //등록
            $('#gnb_03').addClass('on');
            break;

        case 'Check': //점검
            $('#gnb_04').addClass('on');
            break;

        case 'Gojang': //고장
            $('#gnb_05').addClass('on');
            break;

        case 'Maintenance': //유지보수
            $('#gnb_06').addClass('on');
            break;

        // 세팅에 있는 정보 추후 수정예정
        case 'substation':
        case 'member':
        case 'member_app':
            // 추가 작업이 필요하면 여기에 작성
            break;

        default:
            // 기본 설정 추가 가능
            break;
    }

    // 레프트 메뉴 활성화 함수 호출
    activateMenuItem();

    // 현재 페이지와 매칭되는 레프트 메뉴 항목에 'on' 클래스 추가
    function activateMenuItem() {
        $('.menu_group a').each(function () {
            // 현재 링크의 href 속성에서 파일 이름 추출
            const hrefPath = $(this).attr("href").split('/').pop().split('.')[0]; // ex) "VCBList"
            if (lastPageName === hrefPath) {
                $(this).addClass('on'); // 일치하는 메뉴 항목에 'on' 클래스 추가
            }
            else if (leftMenuActivePath.includes('VCB')) {
                $('.menu_group a').eq(0).addClass('on');
            }
            else if (leftMenuActivePath.includes('ITR')) {
                $('.menu_group a').eq(1).addClass('on');
            }
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