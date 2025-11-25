Pick path optimization algorithm using WinForms that generates a near-optimal picking path given user defined pick locations. Major constraint is that backtracking is not allowed.

An order-picker can either
• Travel all the way through the current aisle, picking as necessary, or
• Enter the aisle only as far as necessary to pick all required items therein and then
return to the same end of the aisle from which it was entered.

The picker always travel horizontally from left to right.
User inputs number of aisles and number of pick locations. Thereafter, the user clicks on a storage position to define a pick location. 
Output is the layout and the generated pick path routing.
