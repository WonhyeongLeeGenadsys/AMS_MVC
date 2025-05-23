
//) HI 값에 따라 메시지를 반환하는 헬퍼
function getStrategyMessage(hi, life) {
    if (hi >= 4) {
        return `긴급 유지보수 권장: 상태가 심각하게 악화된 것으로 판단됩니다. 잔여수명은 ${life} 입니다.`;
    }
    if (hi === 3) {
        return `유지보수 계획 권장: 상태가 악화되고 있어 계획적 유지보수가 필요합니다. 잔여수명은 ${life} 입니다.`;
    }
    return `정상 운영 가능: 현재 상태가 양호하여 유지보수가 필요하지 않습니다. 잔여수명은 ${life} 입니다.`;
}

function loadStrategy(url, selector, prefix) {
    const codeKey = `${prefix}_Code`;   
    const $container = $(selector).empty();

    $container.append(`<h2><p>전략 (${prefix})</p></h2>`);

    $.getJSON(url)
        .done(devices => {
            devices.forEach(item => {
                const code = item[codeKey];
                const hi = item.HI;
                const life = item.Remain_Life;
                const msg = getStrategyMessage(hi, life);

                $container.append(
                    `<p><strong>${code}</strong> — ${msg}</p>`
                );
            });
        })
        .fail((_, status, err) => {
            console.error(`loadStrategy(${prefix}) 실패:`, status, err);
            $container.append('<p class="error">전략 데이터를 불러오지 못했습니다.</p>');
        });
}
