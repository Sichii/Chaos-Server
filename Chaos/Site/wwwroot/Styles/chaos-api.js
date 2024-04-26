async function searchItems(params) {
    let response = await fetch(`/items?handler=SearchItems`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}

async function searchSkills(params) {
    let response = await fetch(`/skills?handler=SearchSkills`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}

async function searchSpells(params) {
    let response = await fetch(`/spells?handler=SearchSpells`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}

async function searchMonsters(params) {
    let response = await fetch(`/monsters?handler=SearchMonsters`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    return await response.json();
}