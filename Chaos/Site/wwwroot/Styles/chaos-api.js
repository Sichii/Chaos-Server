async function getItems(params) {
    let response = await fetch(`/items?handler=ItemPage`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}

async function getSkills(params) {
    let response = await fetch(`/skills?handler=SkillPage`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}

async function getSpells(params) {
    let response = await fetch(`/spells?handler=SpellPage`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}