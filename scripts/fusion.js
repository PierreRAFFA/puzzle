const arr = [
    ['I', 'O', 'O', null, 'X', 'X'],
    ['O', 'O', 'O', 'O', 'I', 'X'],
    ['O', 'O', 'O', 'O', 'I', 'O'],
    ['O', 'I', 'O', 'O', 'X', 'I'],
    ['X', 'O', 'X', 'I', 'X', 'O'],
    ['O', 'X', 'X', 'X', 'I', 'I'],
    ['I', 'X', 'I', 'I', 'I', 'I'],
    ['O', 'O', 'X', 'O', 'I', 'I'],
    ['X', 'X', 'X', 'O', 'X', 'X'],
    ['O', 'X', 'O', 'X', 'X', 'I'],
    ['X', 'X', 'X', 'X', 'I', 'I'],
    ['X', 'X', 'X', 'X', 'I', 'I']];
// const arr = generateBlocks(12, 6);

const blocks = convertToBlocks(arr);

// console.log(blocks);
logBlocks(blocks);
//
const unions = findUnions(blocks);
console.log(unions);


function findUnions(blocks) {

    let groups = [];
    let selectedBlocks = [];
    let iR = 0;
    let iC = 0;

    while(iR < blocks.length - 1 && iC < blocks[0].length) {

        // console.log(iR, iC);
        let groupRight = [];
        groupRight = unionRight(blocks, [], iR, iC, 2, selectedBlocks);
        groupRight = unionBottom(blocks, groupRight, iR + 2, iC, groupRight.length, selectedBlocks);

        let groupBottom = [];
        groupBottom = unionBottom(blocks, [], iR, iC, 2, selectedBlocks);
        groupBottom = unionRight(blocks, groupBottom, iR, iC + 2, groupBottom.length, selectedBlocks);

        const flattenGroupRight = flatten(groupRight);
        const flattenGroupBottom = flatten(groupBottom);

        let selectedGroup;
        if (flattenGroupRight.length >= 4 || flattenGroupBottom.length >= 4 ) {
            console.log(iR, iC);
            if (flattenGroupRight.length >= flattenGroupBottom.length) {
                selectedGroup = flattenGroupRight;
                iC = iC + groupRight.length;
            }else{
                selectedGroup = flattenGroupBottom;
                iC = iC + flattenGroupBottom.length / groupBottom.length;
            }


            console.log(selectedGroup);
            // process.exit();
            groups = [...groups, selectedGroup];

            selectedBlocks = [...selectedBlocks, ...selectedGroup];
        }else{
            iC++;
        }

        //boundaries
        if (iC > blocks[0].length - 2) {
            iR++;
            iC = 0;
        }


        // console.log("selectedBlocks:", selectedBlocks);



    }

    return groups;
}


function unionRight(allBlocks, group, row, column, length, selectedBlocks) {
    if (column < allBlocks[0].length && length >= 2) {
        const block = allBlocks[row][column];

        if (block) {
            const color = group.length ? group[0][0].color : block.color;

            let blocks = [];
            let sameColor = true;
            let isOneOfBlocksNull = false;
            let isOneOfBlocksAlreadySelected = false;
            for (let iB = 0; iB < length; iB++) {
                const currentBlock = allBlocks[row + iB][column];
                if (currentBlock) {
                    blocks = [...blocks, currentBlock];
                    // console.log(currentBlock.color);
                    if (currentBlock.color != color) {
                        sameColor = false;
                    }

                    if (selectedBlocks.indexOf(currentBlock) >= 0) {
                        isOneOfBlocksAlreadySelected = true
                    }
                }else{
                    isOneOfBlocksNull = true;
                }

            }

            // console.log(sameColor);
            // console.log(isOneOfBlocksAlreadySelected);
            if (sameColor && isOneOfBlocksAlreadySelected == false&& isOneOfBlocksNull == false) {
                group = [...group, blocks];
                group = unionRight(allBlocks, group, row, column + 1, length, selectedBlocks)
            }
        }

    }

    return group;
}

function unionBottom(allBlocks, group, row, column, length, selectedBlocks) {
    if (row < allBlocks.length  && length >= 2) {
        const block = allBlocks[row][column];

        if (block) {

            const color = group.length ? group[0][0].color : block.color;

            let blocks = [];
            let sameColor = true;
            let isOneOfBlocksNull = false;
            let isOneOfBlocksAlreadySelected = false;
            for (let iB = 0; iB < length; iB++) {
                const currentBlock = allBlocks[row][column + iB];
                if (currentBlock) {
                    blocks = [...blocks, currentBlock];

                    // console.log(allBlocks[row][column + iB].color);
                    if (currentBlock.color != color) {
                        sameColor = false;
                    }

                    if (selectedBlocks.indexOf(currentBlock) >= 0) {
                        isOneOfBlocksAlreadySelected = true
                    }
                }else{
                    isOneOfBlocksNull = true;
                }

            }

            // console.log(sameColor);
            // console.log(isOneOfBlocksAlreadySelected);
            if (sameColor && isOneOfBlocksAlreadySelected == false && isOneOfBlocksNull == false) {
                group = [...group, blocks];
                group = unionBottom(allBlocks, group, row + 1, column, length, selectedBlocks)
            }
        }
    }

    return group;
}


function flatten(arr) {
    let result = [];

    for(let i = 0; i < arr.length; i++) {
        if (Array.isArray(arr[i])) {
            result = [...result, ...flatten(arr[i])];
        }else{
            result = [...result , arr[i]];
        }
    }

    return result;
}


function Block(color) {
    // this.row = row;
    // this.column = column;
    this.color = color;
    // this.selected = false;
}


function convertToBlocks(arr) {
    let result = []
    for (let iR = 0; iR < arr.length; iR++) {
        let row = []
        for (let iC = 0; iC < arr[iR].length; iC++) {
            if (arr[iR][iC]) {
                row = [...row, new Block(arr[iR][iC])]
            }else{
                row = [...row, null]
            }

        }
        result = [...result, row];

    }
    return result;
}

function logBlocks(blocks) {
    let result = "";
    for (let iR = 0; iR < blocks.length; iR++) {
        let row = "";
        for (let iC = 0; iC < blocks[iR].length; iC++) {
            if (blocks[iR][iC]) {
                row += `${blocks[iR][iC].color} `;
            }else{
                row += `  `;
            }

        }
        result += `${row}\n`;
    }
    console.log(result);
}


function generateBlocks(numRows, numColumns) {
    let result = [];
    for (let iR = 0; iR < numRows; iR++) {
        let row = [];
        for (let iC = 0; iC < numColumns; iC++) {
            row = [...row, getRandomBlock()]
        }
        result = [...result, row];
    }

    return result;
}

function getRandomBlock() {
    const colors = ["O", "X", "I"];
    return (colors[Math.round(Math.round(Math.random() * (colors.length - 1)))]);
}