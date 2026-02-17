Pick path optimization algorithm using WinForms that generates a near-optimal picking path given user defined pick locations. Orders are representing picking totes that are put on a unit-load (e.g pallet). The goal is to optimize the allocation and internal order of the totes on the unit-load so not to block for item picks into totes during the picking round (a tote has to be closed to allow for placement of anoter tote on top of it).

Major constraint is that backtracking is not allowed.

An order-picker can either
• Travel all the way through the current aisle, picking as necessary, or
• Enter the aisle only as far as necessary to pick all required items therein and then
return to the same end of the aisle from which it was entered.

The picker always travel horizontally from left to right.
User inputs number of aisles and number of pick locations. Thereafter, the user clicks on a storage position to define a pick location. 
Output is the layout and the generated pick path routing.
