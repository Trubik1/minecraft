// ==============================================================================
//  KubeJS Server Script: Endgame Creative Items -> Ultimate Crafting Table (9x9)
//  Сборка: Steampunk & Swords
//
//  Шесть "основных" креативных предметов собираются только на Ultimate Crafting
//  Table (9x9, tier 4). Каждый требует ДВЕ кастомные сингулярности:
//    - extendedcrafting:niotic  (1000 кристаллов ниотика Powah)
//    - extendedcrafting:ars     (1000 source gem / magebloom Ars Nouveau)
//  Сами сингулярности делаются в Compressor (Extended Crafting).
//
//  Здесь лежат только те два предмета, для которых 9x9-рецепта ещё не было:
//    - create:creative_blaze_cake
//    - mekanism:creative_fluid_tank
//
//  Остальные четыре (creative_motor, creative_energy_cube,
//  creative_controller, creative_storage_disk) уже были 9x9 и обновлены
//  в progression_recipes.js — там же, где лежали изначально.
//
//  ВАЖНО: старые 3x3-рецепты этих предметов удалены, чтобы креатив нельзя
//  было получить в обычном верстаке.
// ==============================================================================

ServerEvents.recipes(event => {
    console.info('=== Loading Endgame 9x9 Creative Recipes ===');

    // Сингулярность Extended Crafting — предмет с компонентом singularity_id.
    const singularity = (id) => ({
        type: 'neoforge:components',
        items: ['extendedcrafting:singularity'],
        components: { 'extendedcrafting:singularity_id': id },
        strict: false
    });

    const niotic = () => singularity('extendedcrafting:niotic');
    const ars = () => singularity('extendedcrafting:ars');

    const table = (id, pattern, key, result) => {
        event.custom({
            type: 'extendedcrafting:shaped_table',
            tier: 4,
            pattern: pattern,
            key: key,
            result: { id: result, count: 1 }
        }).id(id);
    };

    // --------------------------------------------------------------------------
    // 1. КРЕАТИВНЫЙ ТОРТ СПОЛОХА (Create: Creative Blaze Cake)
    // --------------------------------------------------------------------------
    table('steampunk:endgame/creative_blaze_cake', [
        'V  A B   ',
        '         ',
        '  C D E  ',
        '         ',
        'F   G   H',
        '         ',
        '  I J K  ',
        '         ',
        'L       W'
    ], {
        A: { item: 'cataclysm:ignitium_ingot' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'powah:crystal_nitro' },
        D: { item: 'create:blaze_cake' },
        E: { item: 'ars_nouveau:source_gem_block' },
        F: { item: 'mekanism:ultimate_control_circuit' },
        G: { item: 'farmersdelight:roast_chicken_block' },
        H: { item: 'create:precision_mechanism' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'create:blaze_cake_base' },
        K: { item: 'ars_nouveau:fire_essence' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'create:creative_blaze_cake');

    // --------------------------------------------------------------------------
    // 2. КРЕАТИВНЫЙ РЕЗЕРВУАР ЖИДКОСТЕЙ (Mekanism: Creative Fluid Tank)
    // --------------------------------------------------------------------------
    table('steampunk:endgame/creative_fluid_tank', [
        'V  A B   ',
        '         ',
        '  C D E  ',
        '         ',
        'F   G   H',
        '         ',
        '  I J K  ',
        '         ',
        'L       W'
    ], {
        A: { item: 'industrialforegoing:plastic' },
        B: { item: 'cataclysm:void_core' },
        C: { item: 'mekanism:ultimate_fluid_tank' },
        D: { item: 'ars_nouveau:source_jar' },
        E: { item: 'mekanism:ultimate_control_circuit' },
        F: { item: 'create:mechanical_pump' },
        G: { item: 'refinedstorage:advanced_processor' },
        H: { item: 'powah:ender_cell_nitro' },
        I: { item: 'minecraft:nether_star' },
        J: { item: 'industrialforegoing:machine_frame_supreme' },
        K: { item: 'ars_nouveau:source_gem_block' },
        L: { item: 'mekanism:ultimate_energy_cube' },
        V: niotic(),
        W: ars()
    }, 'mekanism:creative_fluid_tank');

    console.info('=== Endgame 9x9 Creative Recipes Loaded Successfully! ===');
});
