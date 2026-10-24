// ==============================================================================
//  KubeJS Server Script: Endgame Mega-Crafts & Creative Items for 1.21.1
//  Сборка: Steampunk & Swords
//  Объединяет Create, Mekanism, Ars Nouveau, Refined Storage, Powah, Cataclysm!
// ==============================================================================

ServerEvents.recipes(event => {
    console.info('=== Loading Endgame Mega-Crafts Recipes ===');

    // --------------------------------------------------------------------------
    // 1. КРЕАТИВНЫЙ МОТОР (Create: Creative Motor)
    // Требует: Броню и реликвию Левиафана/Монструозности, Процессоры Mekanism,
    // Контроллер RS, Батарею Powah и Паровой Двигатель Create
    // --------------------------------------------------------------------------
    event.shaped('create:creative_motor', [
        'ABC',
        'DEF',
        'GHI'
    ], {
        A: 'create:brass_casing',
        B: 'cataclysm:abyssal_sacrifice',
        C: 'create:steam_engine',
        D: 'mekanism:ultimate_control_circuit',
        E: 'create:precision_mechanism',
        F: 'powah:battery_nitro',
        G: 'refinedstorage:controller',
        H: 'ars_nouveau:source_gem_block',
        I: 'industrialforegoing:machine_frame_supreme'
    }).id('steampunk:endgame/creative_motor');

    // --------------------------------------------------------------------------
    // 2. КРЕАТИВНЫЙ ТОРТ СПОЛОХА (Create: Creative Blaze Cake)
    // Бесконечное топливо максимальной температуры для двигателей и паровых котлов
    // --------------------------------------------------------------------------
    event.shaped('create:creative_blaze_cake', [
        ' A ',
        'BCD',
        ' E '
    ], {
        A: 'cataclysm:ignitium_ingot',
        B: 'powah:crystal_nitro',
        C: 'create:blaze_cake',
        D: 'ars_nouveau:source_gem',
        E: 'farmersdelight:roast_chicken_block'
    }).id('steampunk:endgame/creative_blaze_cake');

    // --------------------------------------------------------------------------
    // 3. КРЕАТИВНЫЙ ЭНЕРГЕТИЧЕСКИЙ КУБ (Mekanism: Creative Energy Cube)
    // Бесконечная энергия любого вольтажа
    // --------------------------------------------------------------------------
    event.shaped('mekanism:creative_energy_cube', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'fluxnetworks:gargantuan_flux_storage',
        B: 'powah:ender_cell_nitro',
        C: 'mekanism:ultimate_control_circuit',
        D: 'mekanism:ultimate_energy_cube',
        E: 'cataclysm:witherite_ingot'
    }).id('steampunk:endgame/creative_energy_cube');

    // --------------------------------------------------------------------------
    // 4. КРЕАТИВНЫЙ РЕЗЕРВУАР ЖИДКОСТЕЙ (Mekanism: Creative Fluid Tank)
    // Бесконечный источник любой залитой жидкости или газа
    // --------------------------------------------------------------------------
    event.shaped('mekanism:creative_fluid_tank', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'industrialforegoing:plastic',
        B: 'cataclysm:void_core',
        C: 'mekanism:ultimate_control_circuit',
        D: 'mekanism:ultimate_fluid_tank',
        E: 'ars_nouveau:source_jar'
    }).id('steampunk:endgame/creative_fluid_tank');

    // --------------------------------------------------------------------------
    // 5. КРЕАТИВНЫЙ ДИСК ХРАНИЛИЩА (Refined Storage: Creative Storage Disk)
    // Бесконечное пространство для любых предметов цифровой сети
    // --------------------------------------------------------------------------
    event.shaped('refinedstorage:creative_storage_disk', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'refinedstorage:advanced_processor',
        B: 'cataclysm:void_core',
        C: 'create:precision_mechanism',
        D: 'refinedstorage:64k_storage_disk',
        E: 'mekanism:teleportation_core'
    }).id('steampunk:endgame/creative_storage_disk');

    // --------------------------------------------------------------------------
    // 6. КРЕАТИВНЫЙ КОНТРОЛЛЕР (Refined Storage: Creative Controller)
    // Бесконечная энергия для МЭ-сети
    // --------------------------------------------------------------------------
    event.shaped('refinedstorage:creative_controller', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'refinedstorage:advanced_processor',
        B: 'cataclysm:witherite_ingot',
        C: 'powah:player_transmitter_nitro',
        D: 'refinedstorage:controller',
        E: 'create:creative_motor'
    }).id('steampunk:endgame/creative_controller');

    console.info('=== Endgame Mega-Crafts Recipes Loaded Successfully! ===');
});
