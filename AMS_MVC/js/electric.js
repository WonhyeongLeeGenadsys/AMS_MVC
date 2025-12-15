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

    });
});

// 현재 달러 환율 API 원화(억원)값 적용
let usdKrwRate = 0;

function loadUsdKrwRate() {
    const url = document.body.getAttribute('data-usdkrw-url'); 

    return $.getJSON(url)
        .done(r => {
            usdKrwRate = r.rate || 0;
            console.log('USD/KRW:', usdKrwRate);
        })
        .fail(xhr => {
            usdKrwRate = 0;
            console.error('환율 로드 실패', xhr.status, xhr.responseText);
        });
}

//의사결정 탭에서 POF*COF값에 대한 값 
function getDMDecision(value) {
    if (value >= 50) return { text: '긴급 유지보수', color: 'red' };
    if (value >= 40) return { text: '즉시 교체', color: 'orange' };
    if (value >= 30) return { text: '예방 정비', color: 'gold' };
    if (value >= 20) return { text: '정기 점검', color: '#b4deb1' };
    if (value >= 10) return { text: '지속 감시', color: '#2e8b57' };
    return { text: '-', color: '#999' };
}



