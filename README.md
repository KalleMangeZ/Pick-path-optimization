Pick path optimization algorithm using WinForms that generates a near-optimal picking path given user defined pick locations for items. Orders are representing picking bin that are put on a unit-load (e.g pallet). Items that need to be picked in the same bin are marked with the same number, and all items need to be picked in the pick order. The goal is to optimize the allocation and internal order of the bins on the unit-load so not to block/disable accesibility for item picks into unfinished bins during the picking round (a bin has to be closed to allow for placement of anoter bin on top of it).

Major constraint is that backtracking is not allowed.

An order-picker can either
• Travel all the way through the current aisle, picking as necessary, or
• Enter the aisle only as far as necessary to pick all required items therein and then
return to the same end of the aisle from which it was entered.

The picker always travel horizontally from left to right.
User inputs number of aisles and number of pick locations. Thereafter, the user clicks on a storage position to define a pick location. 
Output is the layout and the generated pick path routing.
