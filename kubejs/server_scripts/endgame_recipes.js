// ==============================================================================
//  KubeJS Server Script: Complete Endgame Creative Item Recipes
//  Сборка: Steampunk & Swords
//
//  Полная линейка крафтов для ВСЕХ креативных предметов сборки:
//  - Create, Create Addons & Big Cannons
//  - Mekanism & Mekanism Integration
//  - Refined Storage & Cable Tiers
//  - Ars Nouveau
//  - PneumaticCraft
//  - Mystical Agriculture & Agradditions
//  - Functional Storage & Storage Drawers
//  - Titanium
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
            pattern: pattern,
            key: key,
            result: { id: result, count: 1 }
        }).id(id);
    };

    // ──────────────────────────────────────────────────────────────────────────
    // 1. CREATE & CREATE ADDONS
    // ──────────────────────────────────────────────────────────────────────────

    // 1.1 Креативный торт сполоха (Create: Creative Blaze Cake)
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

    // 1.2 Креативный резервуар жидкостей (Create: Creative Fluid Tank)
    table('steampunk:endgame/creative_fluid_tank_create', [
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
        A: { item: 'create:fluid_tank' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'create:mechanical_pump' },
        D: { item: 'mekanism:ultimate_fluid_tank' },
        E: { item: 'ars_nouveau:water_essence' },
        F: { item: 'create:precision_mechanism' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'create:fluid_tank' },
        K: { item: 'industrialforegoing:plastic' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'create:creative_fluid_tank');

    // 1.3 Креативный ящик (Create: Creative Crate)
    table('steampunk:endgame/creative_crate', [
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
        A: { item: 'create:brass_casing' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'create:precision_mechanism' },
        D: { item: 'functionalstorage:diamond_upgrade' },
        E: { item: 'create:precision_mechanism' },
        F: { item: 'minecraft:netherite_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'minecraft:netherite_block' },
        I: { item: 'industrialforegoing:plastic' },
        J: { item: 'create:brass_casing' },
        K: { item: 'twilightforest:carminite' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'create:creative_crate');

    // 1.4 Креативная энергия (Create Crafts & Additions: Creative Energy)
    table('steampunk:endgame/creative_energy_createaddition', [
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
        A: { item: 'createaddition:electric_motor' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'createaddition:alternator' },
        D: { item: 'powah:ender_cell_nitro' },
        E: { item: 'createaddition:tesla_coil' },
        F: { item: 'create:precision_mechanism' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'mekanism:ultimate_energy_cube' },
        I: { item: 'powah:crystal_nitro' },
        J: { item: 'createaddition:electric_motor' },
        K: { item: 'twilightforest:carminite' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'createaddition:creative_energy');

    // ──────────────────────────────────────────────────────────────────────────
    // 2. MEKANISM
    // ──────────────────────────────────────────────────────────────────────────

    // 2.1 Креативный резервуар жидкостей Mekanism
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

    // 2.2 Креативный химический бак (Mekanism: Creative Chemical Tank)
    table('steampunk:endgame/creative_chemical_tank', [
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
        A: { item: 'mekanism:ultimate_chemical_tank' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'mekanism:pellet_antimatter' },
        D: { item: 'mekanism:ultimate_chemical_tank' },
        E: { item: 'mekanism:ultimate_control_circuit' },
        F: { item: 'cataclysm:void_core' },
        G: { item: 'powah:crystal_nitro' },
        H: { item: 'industrialforegoing:machine_frame_supreme' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'mekanism:ultimate_chemical_tank' },
        K: { item: 'ars_nouveau:source_gem_block' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'mekanism:creative_chemical_tank');

    // 2.3 Креативный контейнер (Mekanism: Creative Bin)
    table('steampunk:endgame/creative_bin', [
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
        A: { item: 'mekanism:ultimate_bin' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'mekanism:pellet_antimatter' },
        D: { item: 'mekanism:ultimate_bin' },
        E: { item: 'mekanism:ultimate_control_circuit' },
        F: { item: 'minecraft:netherite_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'create:precision_mechanism' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'mekanism:ultimate_bin' },
        K: { item: 'industrialforegoing:plastic' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'mekanism:creative_bin');

    // ──────────────────────────────────────────────────────────────────────────
    // 3. ARS NOUVEAU
    // ──────────────────────────────────────────────────────────────────────────

    // 3.1 Креативный кувшин источника (Ars Nouveau: Creative Source Jar)
    table('steampunk:endgame/creative_source_jar', [
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
        A: { item: 'ars_nouveau:source_gem_block' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'ars_nouveau:fire_essence' },
        D: { item: 'ars_nouveau:source_jar' },
        E: { item: 'ars_nouveau:water_essence' },
        F: { item: 'ars_nouveau:archmage_spell_book' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'ars_nouveau:earth_essence' },
        J: { item: 'ars_nouveau:source_gem_block' },
        K: { item: 'ars_nouveau:air_essence' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'ars_nouveau:creative_source_jar');

    // 3.2 Креативная книга заклинаний (Ars Nouveau: Creative Spell Book)
    table('steampunk:endgame/creative_spell_book', [
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
        A: { item: 'ars_nouveau:wilden_tribute' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'cataclysm:ignitium_ingot' },
        D: { item: 'ars_nouveau:archmage_spell_book' },
        E: { item: 'ars_nouveau:source_gem_block' },
        F: { item: 'twilightforest:carminite' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'ars_nouveau:fire_essence' },
        J: { item: 'ars_nouveau:wilden_tribute' },
        K: { item: 'ars_nouveau:water_essence' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'ars_nouveau:creative_spell_book');

    // ──────────────────────────────────────────────────────────────────────────
    // 4. REFINED STORAGE (ENDGAME DRIVES & ADAPTERS)
    // ──────────────────────────────────────────────────────────────────────────

    // 4.1 Креативный жидкостный накопитель (Refined Storage: Creative Fluid Storage Disk)
    table('steampunk:endgame/creative_fluid_storage_disk', [
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
        A: { item: 'refinedstorage:4096b_fluid_storage_disk' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'refinedstorage:advanced_processor' },
        D: { item: 'create:creative_fluid_tank' },
        E: { item: 'refinedstorage:advanced_processor' },
        F: { item: 'mekanism:creative_fluid_tank' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'refinedstorage:storage_housing' },
        K: { item: 'ars_nouveau:water_essence' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'refinedstorage:creative_fluid_storage_disk');

    // 4.2 Креативный химический накопитель (Refined Storage Mekanism Integration)
    table('steampunk:endgame/creative_chemical_storage_disk', [
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
        A: { item: 'refinedstorage:storage_housing' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'refinedstorage:advanced_processor' },
        D: { item: 'mekanism:creative_chemical_tank' },
        E: { item: 'refinedstorage:advanced_processor' },
        F: { item: 'mekanism:pellet_antimatter' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'mekanism:ultimate_control_circuit' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'refinedstorage:storage_housing' },
        K: { item: 'industrialforegoing:plastic' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'refinedstorage_mekanism_integration:creative_chemical_storage_disk');

    // ──────────────────────────────────────────────────────────────────────────
    // 5. PNEUMATICCRAFT
    // ──────────────────────────────────────────────────────────────────────────

    // 5.1 Креативный компрессор (PneumaticCraft: Creative Compressor)
    table('steampunk:endgame/creative_compressor', [
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
        A: { item: 'pneumaticcraft:advanced_pressure_tube' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'pneumaticcraft:pressure_chamber_wall' },
        D: { item: 'pneumaticcraft:advanced_air_compressor' },
        E: { item: 'pneumaticcraft:pressure_chamber_wall' },
        F: { item: 'pneumaticcraft:compressed_iron_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'create:precision_mechanism' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'pneumaticcraft:advanced_pressure_tube' },
        K: { item: 'powah:crystal_nitro' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'pneumaticcraft:creative_compressor');

    // 5.2 Креативный блок сжатого железа (PneumaticCraft: Creative Compressed Iron Block)
    table('steampunk:endgame/creative_compressed_iron_block', [
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
        A: { item: 'pneumaticcraft:compressed_iron_block' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'pneumaticcraft:compressed_iron_block' },
        D: { item: 'minecraft:netherite_block' },
        E: { item: 'pneumaticcraft:compressed_iron_block' },
        F: { item: 'pneumaticcraft:compressed_iron_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'pneumaticcraft:compressed_iron_block' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'pneumaticcraft:compressed_iron_block' },
        K: { item: 'powah:crystal_nitro' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'pneumaticcraft:creative_compressed_iron_block');

    // ──────────────────────────────────────────────────────────────────────────
    // 6. FUNCTIONAL STORAGE & STORAGE DRAWERS
    // ──────────────────────────────────────────────────────────────────────────

    // 6.1 Креативное улучшение выдачи (Functional Storage: Creative Vending Upgrade)
    table('steampunk:endgame/creative_vending_upgrade_fs', [
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
        A: { item: 'functionalstorage:diamond_upgrade' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'create:creative_crate' },
        D: { item: 'functionalstorage:netherite_upgrade' },
        E: { item: 'create:creative_crate' },
        F: { item: 'minecraft:netherite_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'functionalstorage:diamond_upgrade' },
        K: { item: 'ars_nouveau:source_gem_block' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'functionalstorage:creative_vending_upgrade');

    // 6.2 Креативное улучшение выдачи (Storage Drawers: Creative Vending Upgrade)
    table('steampunk:endgame/creative_vending_upgrade_sd', [
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
        A: { item: 'storagedrawers:emerald_storage_upgrade' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'create:creative_crate' },
        D: { item: 'storagedrawers:upgrade_template' },
        E: { item: 'create:creative_crate' },
        F: { item: 'minecraft:netherite_block' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'storagedrawers:emerald_storage_upgrade' },
        K: { item: 'ars_nouveau:source_gem_block' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'storagedrawers:creative_vending_upgrade');

    // ──────────────────────────────────────────────────────────────────────────
    // 7. MYSTICAL AGRICULTURE & TITANIUM
    // ──────────────────────────────────────────────────────────────────────────

    // 7.1 Креативная эссенция (Mystical Agradditions: Creative Essence)
    table('steampunk:endgame/creative_essence', [
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
        A: { item: 'mysticalagriculture:awakened_supremium_block' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'mysticalagriculture:master_infusion_crystal' },
        D: { item: 'mysticalagradditions:insanium_block' },
        E: { item: 'mysticalagriculture:master_infusion_crystal' },
        F: { item: 'cataclysm:void_core' },
        G: { item: 'mysticalagriculture:awakened_supremium_block' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'mysticalagradditions:insanium_block' },
        K: { item: 'ars_nouveau:source_gem_block' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'mysticalagradditions:creative_essence');

    // 7.2 Креативный кинжал (Mystical Agriculture: Creative Soulium Dagger)
    table('steampunk:endgame/creative_soulium_dagger', [
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
        A: { item: 'mysticalagradditions:creative_essence' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'cataclysm:ignitium_ingot' },
        D: { item: 'mysticalagriculture:soulium_dagger' },
        E: { item: 'mysticalagriculture:awakened_supremium_ingot' },
        F: { item: 'twilightforest:carminite' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'ars_nouveau:fire_essence' },
        J: { item: 'mysticalagradditions:creative_essence' },
        K: { item: 'ars_nouveau:earth_essence' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'mysticalagriculture:creative_soulium_dagger');

    // 7.3 Креативный генератор FE (Titanium: Creative FE Generator)
    table('steampunk:endgame/creative_fe_generator', [
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
        A: { item: 'industrialforegoing:machine_frame_supreme' },
        B: { item: 'minecraft:nether_star' },
        C: { item: 'mekanism:creative_energy_cube' },
        D: { item: 'powah:energy_cell_creative' },
        E: { item: 'createaddition:creative_energy' },
        F: { item: 'industrialforegoing:plastic' },
        G: { item: 'cataclysm:void_core' },
        H: { item: 'powah:crystal_nitro' },
        I: { item: 'twilightforest:carminite' },
        J: { item: 'industrialforegoing:machine_frame_supreme' },
        K: { item: 'refinedstorage:advanced_processor' },
        L: { item: 'mekanism:pellet_antimatter' },
        V: niotic(),
        W: ars()
    }, 'titanium:creative_fe_generator');

    // ──────────────────────────────────────────────────────────────────────────
    // 8. КРАФТЫ В ВЕРСТАКЕ (CRAFTING TABLE SHAPED & SHAPELESS)
    // ──────────────────────────────────────────────────────────────────────────

    // 8.1 Блоки хранения Refined Storage
    event.shaped('refinedstorage:creative_storage_block', [
        'HSH',
        'SDS',
        'HSH'
    ], {
        H: 'refinedstorage:storage_housing',
        S: 'refinedstorage:quartz_enriched_iron',
        D: 'refinedstorage:creative_storage_disk'
    }).id('steampunk:endgame/creative_storage_block');

    event.shaped('refinedstorage:creative_fluid_storage_block', [
        'HSH',
        'SDS',
        'HSH'
    ], {
        H: 'refinedstorage:storage_housing',
        S: 'refinedstorage:quartz_enriched_iron',
        D: 'refinedstorage:creative_fluid_storage_disk'
    }).id('steampunk:endgame/creative_fluid_storage_block');

    event.shaped('refinedstorage_mekanism_integration:creative_chemical_storage_block', [
        'HSH',
        'SDS',
        'HSH'
    ], {
        H: 'refinedstorage:storage_housing',
        S: 'refinedstorage:quartz_enriched_iron',
        D: 'refinedstorage_mekanism_integration:creative_chemical_storage_disk'
    }).id('steampunk:endgame/creative_chemical_storage_block');

    // 8.2 Беспроводные устройства Refined Storage
    event.shaped('refinedstorage:creative_wireless_grid', [
        'NAN',
        'AGA',
        'NAN'
    ], {
        N: 'minecraft:nether_star',
        A: 'refinedstorage:advanced_processor',
        G: 'refinedstorage:wireless_grid'
    }).id('steampunk:endgame/creative_wireless_grid');

    event.shapeless('refinedstorage:creative_wireless_autocrafting_monitor', [
        'refinedstorage:wireless_autocrafting_monitor',
        'refinedstorage:creative_wireless_grid',
        'refinedstorage:advanced_processor'
    ]).id('steampunk:endgame/creative_wireless_autocrafting_monitor');

    event.shapeless('refinedstorage:creative_portable_grid', [
        'refinedstorage:portable_grid',
        'refinedstorage:creative_wireless_grid'
    ]).id('steampunk:endgame/creative_portable_grid');

    event.shapeless('refinedstorage_quartz_arsenal:creative_wireless_crafting_grid', [
        'refinedstorage_quartz_arsenal:wireless_crafting_grid',
        'refinedstorage:creative_wireless_grid'
    ]).id('steampunk:endgame/creative_wireless_crafting_grid');

    event.shapeless('universalgrid:creative_wireless_universal_grid', [
        'universalgrid:wireless_universal_grid',
        'refinedstorage:creative_wireless_grid'
    ]).id('steampunk:endgame/creative_wireless_universal_grid');

    // Улучшение дальности (Creative Range Upgrade) - сбалансированный крафт
    event.shaped('refinedstorage:creative_range_upgrade', [
        'RUR',
        'UNU',
        'RUR'
    ], {
        R: 'refinedstorage:range_upgrade',
        U: 'refinedstorage:upgrade',
        N: 'minecraft:nether_star'
    }).id('steampunk:endgame/creative_range_upgrade');

    // Альтернативный умеренный крафт через Незерит и Улучшенные процессоры (не требующий звезд)
    event.shaped('refinedstorage:creative_range_upgrade', [
        'RPR',
        'PNP',
        'RPR'
    ], {
        R: 'refinedstorage:range_upgrade',
        P: 'refinedstorage:advanced_processor',
        N: 'minecraft:netherite_ingot'
    }).id('steampunk:endgame/creative_range_upgrade_netherite');

    // Двусторонняя конвертация творческих резервуаров Create <-> Mekanism
    event.shapeless('create:creative_fluid_tank', ['mekanism:creative_fluid_tank']).id('steampunk:endgame/convert_mek_to_create_tank');
    event.shapeless('mekanism:creative_fluid_tank', ['create:creative_fluid_tank']).id('steampunk:endgame/convert_create_to_mek_tank');

    // Дополнительный удобный крафт для Творческого беспроводного терминала создания
    event.shapeless('refinedstorage_quartz_arsenal:creative_wireless_crafting_grid', [
        'refinedstorage:creative_wireless_grid',
        'minecraft:crafting_table',
        'refinedstorage:advanced_processor'
    ]).id('steampunk:endgame/creative_wireless_crafting_grid_from_table');

    // 8.3 Креативные надстройки для Create, Storage Drawers, PneumaticCraft
    event.shapeless('create_connected:creative_fluid_vessel', [
        'create_connected:fluid_vessel',
        'create:creative_fluid_tank'
    ]).id('steampunk:endgame/creative_fluid_vessel');

    event.shapeless('createbigcannons:creative_autocannon_ammo_container', [
        'createbigcannons:autocannon_ammo_container',
        'create:creative_crate'
    ]).id('steampunk:endgame/creative_autocannon_ammo_container');

    event.shapeless('storagedrawers:creative_storage_upgrade', [
        'storagedrawers:upgrade_template',
        'storagedrawers:creative_vending_upgrade'
    ]).id('steampunk:endgame/creative_storage_upgrade');

    event.shaped('pneumaticcraft:creative_upgrade', [
        ' M ',
        'MNM',
        ' M '
    ], {
        M: 'pneumaticcraft:upgrade_matrix',
        N: 'pneumaticcraft:creative_compressor'
    }).id('steampunk:endgame/creative_upgrade_pneumatic');

    // 8.4 Cable Tiers (Креативные кабели и интерфейсы)
    const cableDevices = [
        ['autocrafter', 'cabletiers:ultra_autocrafter', 'cabletiers:creative_autocrafter'],
        ['constructor', 'cabletiers:ultra_constructor', 'cabletiers:creative_constructor'],
        ['destructor', 'cabletiers:ultra_destructor', 'cabletiers:creative_destructor'],
        ['disk_interface', 'cabletiers:ultra_disk_interface', 'cabletiers:creative_disk_interface'],
        ['exporter', 'cabletiers:ultra_exporter', 'cabletiers:creative_exporter'],
        ['importer', 'cabletiers:ultra_importer', 'cabletiers:creative_importer'],
        ['interface', 'cabletiers:ultra_interface', 'cabletiers:creative_interface']
    ];

    cableDevices.forEach(([name, ultra, creative]) => {
        event.shapeless(creative, [
            ultra,
            'refinedstorage:creative_controller',
            'refinedstorage:advanced_processor'
        ]).id(`steampunk:endgame/cabletiers_${name}`);
    });

    // 8.5 Перекраска креативного контроллера Refined Storage во все цвета
    const rsColors = [
        'white', 'orange', 'magenta', 'yellow', 'lime', 'pink', 'gray',
        'light_gray', 'cyan', 'purple', 'blue', 'brown', 'green', 'red', 'black'
    ];

    rsColors.forEach(color => {
        event.shapeless(`refinedstorage:${color}_creative_controller`, [
            'refinedstorage:creative_controller',
            `minecraft:${color}_dye`
        ]).id(`steampunk:endgame/creative_controller_${color}`);
    });

    // 8.6 Перекраска устройств Cable Tiers в цветные вариации
    const cableColored = [
        ['cabletiers:black_creative_autocrafter', 'black', 'cabletiers:creative_autocrafter'],
        ['cabletiers:black_creative_constructor', 'black', 'cabletiers:creative_constructor'],
        ['cabletiers:black_creative_destructor', 'black', 'cabletiers:creative_destructor'],
        ['cabletiers:black_creative_disk_interface', 'black', 'cabletiers:creative_disk_interface'],
        ['cabletiers:black_creative_exporter', 'black', 'cabletiers:creative_exporter'],
        ['cabletiers:black_creative_importer', 'black', 'cabletiers:creative_importer'],
        ['cabletiers:blue_creative_autocrafter', 'blue', 'cabletiers:creative_autocrafter'],
        ['cabletiers:blue_creative_constructor', 'blue', 'cabletiers:creative_constructor'],
        ['cabletiers:blue_creative_destructor', 'blue', 'cabletiers:creative_destructor'],
        ['cabletiers:blue_creative_disk_interface', 'blue', 'cabletiers:creative_disk_interface'],
        ['cabletiers:blue_creative_exporter', 'blue', 'cabletiers:creative_exporter'],
        ['cabletiers:blue_creative_importer', 'blue', 'cabletiers:creative_importer'],
        ['cabletiers:brown_creative_autocrafter', 'brown', 'cabletiers:creative_autocrafter'],
        ['cabletiers:brown_creative_constructor', 'brown', 'cabletiers:creative_constructor'],
        ['cabletiers:brown_creative_destructor', 'brown', 'cabletiers:creative_destructor'],
        ['cabletiers:brown_creative_disk_interface', 'brown', 'cabletiers:creative_disk_interface'],
        ['cabletiers:brown_creative_exporter', 'brown', 'cabletiers:creative_exporter'],
        ['cabletiers:brown_creative_importer', 'brown', 'cabletiers:creative_importer'],
        ['cabletiers:cyan_creative_autocrafter', 'cyan', 'cabletiers:creative_autocrafter'],
        ['cabletiers:cyan_creative_constructor', 'cyan', 'cabletiers:creative_constructor'],
        ['cabletiers:cyan_creative_destructor', 'cyan', 'cabletiers:creative_destructor'],
        ['cabletiers:cyan_creative_disk_interface', 'cyan', 'cabletiers:creative_disk_interface'],
        ['cabletiers:cyan_creative_exporter', 'cyan', 'cabletiers:creative_exporter'],
        ['cabletiers:cyan_creative_importer', 'cyan', 'cabletiers:creative_importer'],
        ['cabletiers:gray_creative_autocrafter', 'gray', 'cabletiers:creative_autocrafter'],
        ['cabletiers:gray_creative_disk_interface', 'gray', 'cabletiers:creative_disk_interface'],
        ['cabletiers:green_creative_autocrafter', 'green', 'cabletiers:creative_autocrafter'],
        ['cabletiers:green_creative_constructor', 'green', 'cabletiers:creative_constructor'],
        ['cabletiers:green_creative_destructor', 'green', 'cabletiers:creative_destructor'],
        ['cabletiers:green_creative_disk_interface', 'green', 'cabletiers:creative_disk_interface'],
        ['cabletiers:green_creative_exporter', 'green', 'cabletiers:creative_exporter'],
        ['cabletiers:green_creative_importer', 'green', 'cabletiers:creative_importer'],
        ['cabletiers:light_blue_creative_constructor', 'light_blue', 'cabletiers:creative_constructor'],
        ['cabletiers:light_blue_creative_destructor', 'light_blue', 'cabletiers:creative_destructor'],
        ['cabletiers:light_blue_creative_exporter', 'light_blue', 'cabletiers:creative_exporter'],
        ['cabletiers:light_blue_creative_importer', 'light_blue', 'cabletiers:creative_importer'],
        ['cabletiers:light_gray_creative_autocrafter', 'light_gray', 'cabletiers:creative_autocrafter'],
        ['cabletiers:light_gray_creative_constructor', 'light_gray', 'cabletiers:creative_constructor'],
        ['cabletiers:light_gray_creative_destructor', 'light_gray', 'cabletiers:creative_destructor'],
        ['cabletiers:light_gray_creative_disk_interface', 'light_gray', 'cabletiers:creative_disk_interface'],
        ['cabletiers:light_gray_creative_exporter', 'light_gray', 'cabletiers:creative_exporter'],
        ['cabletiers:light_gray_creative_importer', 'light_gray', 'cabletiers:creative_importer'],
        ['cabletiers:lime_creative_autocrafter', 'lime', 'cabletiers:creative_autocrafter'],
        ['cabletiers:lime_creative_constructor', 'lime', 'cabletiers:creative_constructor'],
        ['cabletiers:lime_creative_destructor', 'lime', 'cabletiers:creative_destructor'],
        ['cabletiers:lime_creative_disk_interface', 'lime', 'cabletiers:creative_disk_interface'],
        ['cabletiers:lime_creative_exporter', 'lime', 'cabletiers:creative_exporter'],
        ['cabletiers:lime_creative_importer', 'lime', 'cabletiers:creative_importer'],
        ['cabletiers:magenta_creative_autocrafter', 'magenta', 'cabletiers:creative_autocrafter'],
        ['cabletiers:magenta_creative_constructor', 'magenta', 'cabletiers:creative_constructor'],
        ['cabletiers:magenta_creative_destructor', 'magenta', 'cabletiers:creative_destructor'],
        ['cabletiers:magenta_creative_disk_interface', 'magenta', 'cabletiers:creative_disk_interface'],
        ['cabletiers:magenta_creative_exporter', 'magenta', 'cabletiers:creative_exporter'],
        ['cabletiers:magenta_creative_importer', 'magenta', 'cabletiers:creative_importer'],
        ['cabletiers:orange_creative_autocrafter', 'orange', 'cabletiers:creative_autocrafter'],
        ['cabletiers:orange_creative_constructor', 'orange', 'cabletiers:creative_constructor'],
        ['cabletiers:orange_creative_destructor', 'orange', 'cabletiers:creative_destructor'],
        ['cabletiers:orange_creative_disk_interface', 'orange', 'cabletiers:creative_disk_interface'],
        ['cabletiers:orange_creative_exporter', 'orange', 'cabletiers:creative_exporter'],
        ['cabletiers:orange_creative_importer', 'orange', 'cabletiers:creative_importer'],
        ['cabletiers:pink_creative_autocrafter', 'pink', 'cabletiers:creative_autocrafter'],
        ['cabletiers:pink_creative_constructor', 'pink', 'cabletiers:creative_constructor'],
        ['cabletiers:pink_creative_destructor', 'pink', 'cabletiers:creative_destructor'],
        ['cabletiers:pink_creative_disk_interface', 'pink', 'cabletiers:creative_disk_interface'],
        ['cabletiers:pink_creative_exporter', 'pink', 'cabletiers:creative_exporter'],
        ['cabletiers:pink_creative_importer', 'pink', 'cabletiers:creative_importer'],
        ['cabletiers:purple_creative_autocrafter', 'purple', 'cabletiers:creative_autocrafter'],
        ['cabletiers:purple_creative_constructor', 'purple', 'cabletiers:creative_constructor'],
        ['cabletiers:purple_creative_destructor', 'purple', 'cabletiers:creative_destructor'],
        ['cabletiers:purple_creative_disk_interface', 'purple', 'cabletiers:creative_disk_interface'],
        ['cabletiers:purple_creative_exporter', 'purple', 'cabletiers:creative_exporter'],
        ['cabletiers:purple_creative_importer', 'purple', 'cabletiers:creative_importer'],
        ['cabletiers:red_creative_autocrafter', 'red', 'cabletiers:creative_autocrafter'],
        ['cabletiers:red_creative_constructor', 'red', 'cabletiers:creative_constructor'],
        ['cabletiers:red_creative_destructor', 'red', 'cabletiers:creative_destructor'],
        ['cabletiers:red_creative_disk_interface', 'red', 'cabletiers:creative_disk_interface'],
        ['cabletiers:red_creative_exporter', 'red', 'cabletiers:creative_exporter'],
        ['cabletiers:red_creative_importer', 'red', 'cabletiers:creative_importer'],
        ['cabletiers:white_creative_autocrafter', 'white', 'cabletiers:creative_autocrafter'],
        ['cabletiers:white_creative_constructor', 'white', 'cabletiers:creative_constructor'],
        ['cabletiers:white_creative_destructor', 'white', 'cabletiers:creative_destructor'],
        ['cabletiers:white_creative_disk_interface', 'white', 'cabletiers:creative_disk_interface'],
        ['cabletiers:white_creative_exporter', 'white', 'cabletiers:creative_exporter'],
        ['cabletiers:white_creative_importer', 'white', 'cabletiers:creative_importer'],
        ['cabletiers:yellow_creative_autocrafter', 'yellow', 'cabletiers:creative_autocrafter'],
        ['cabletiers:yellow_creative_constructor', 'yellow', 'cabletiers:creative_constructor'],
        ['cabletiers:yellow_creative_destructor', 'yellow', 'cabletiers:creative_destructor'],
        ['cabletiers:yellow_creative_disk_interface', 'yellow', 'cabletiers:creative_disk_interface'],
        ['cabletiers:yellow_creative_exporter', 'yellow', 'cabletiers:creative_exporter'],
        ['cabletiers:yellow_creative_importer', 'yellow', 'cabletiers:creative_importer'],
    ];

    cableColored.forEach(([result, color, base]) => {
        event.shapeless(result, [base, `minecraft:${color}_dye`]).id(`steampunk:endgame/${result.replace(':', '_')}`);
    });

    console.info('=== Endgame 9x9 Creative Recipes Loaded Successfully! ===');
});
