LzBible.Init();

function DefaultInit(done){
    setTimeout(done,600);
}

describe("Initialize", function () {
  beforeAll(DefaultInit);
  //afterAll(d=> setTimeout(d,1000));

  it("Initialize Bible", function () {
    expect(LzBible.Bibles()).toBeDefined();
    expect(LzBible.Bibles().length).toBeGreaterThan(0);
  });
});

describe("Load Bible Index", function () {
  beforeAll(done=>{
    var bibles = LzBible.Bibles();
    var info = $(LzBible.Bibles())[0];
    LzBible.GetBibleInfo(info.iso,info.abbr);
    setTimeout(done,1000);
  });
  
  it("Load First Bible", function () {
    var bible = LzBible.Bible();
    expect(bible).toBeDefined();
    
  });
});

describe("Load Bible Chapter", function () {
  var chapterData;

  beforeAll(done=>{
    var bibles = LzBible.Bibles();
    var info = $(LzBible.Bibles())[0];
    LzBible.GetBibleInfo(info.iso,info.abbr);

    LzBible.GetChapter('id','tb',1,1,d=>{
      chapterData = d;
    })
    setTimeout(done,1000);
  });
  
  it("Load First Chapter", function () {
    
    expect(chapterData).toBeDefined();
    
  });
});


